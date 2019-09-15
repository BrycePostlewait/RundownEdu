﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace RundownEdu.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Generates a GUID-based editor template, rather than the index-based template generated by Html.EditorFor()
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="propertyExpression">An expression which points to the property on the model you wish to generate the editor for</param>
        /// <param name="indexResolverExpression">An expression which points to the property on the model which holds the GUID index (optional, but required to make Validation* methods to work on post-back)</param>
        /// <param name="includeIndexField">
        /// True if you want this helper to render the hidden &lt;input /&gt; for you (default). False if you do not want this behaviour, and are instead going to call Html.EditorForManyIndexField() within the Editor view. 
        /// The latter behaviour is desired in situations where the Editor is being rendered inside lists or tables, where the &lt;input /&gt; would be invalid.
        /// </param>
        /// <returns>Generated HTML</returns>
        public static IHtmlContent EditorForMany<TModel, TValue>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, IEnumerable<TValue>>> propertyExpression,
            Expression<Func<TValue, string>> indexResolverExpression = null, bool includeIndexField = true)
            where TModel : class
        {
            var items = propertyExpression.Compile()(html.ViewData.Model);
            var htmlBuilder = new StringBuilder();
            var htmlFieldName = ExpressionHelper.GetExpressionText(propertyExpression);
            var htmlFieldNameWithPrefix = html.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            Func<TValue, string> indexResolver;

            if (indexResolverExpression == null)
            {
                indexResolver = x => null;
            }
            else
            {
                indexResolver = indexResolverExpression.Compile();
            }

            foreach (var item in items)
            {
                var dummy = new { Item = item };
                var guid = indexResolver(item);
                var memberExp = Expression.MakeMemberAccess(Expression.Constant(dummy), dummy.GetType().GetProperty("Item"));
                var singleItemExp = Expression.Lambda<Func<TModel, TValue>>(memberExp, propertyExpression.Parameters);

                if (string.IsNullOrEmpty(guid))
                {
                    guid = Guid.NewGuid().ToString();
                }
                else
                {
                    guid = WebUtility.HtmlEncode(guid);
                }

                if (includeIndexField)
                {
                    IHtmlContent indexHtml = _EditorForManyIndexField(htmlFieldNameWithPrefix, guid, indexResolverExpression);
                    AppendHtml(indexHtml, htmlBuilder);
                }

                IHtmlContent itemHtml = html.EditorFor(singleItemExp, null, $"{htmlFieldName}[{guid}]");
                AppendHtml(itemHtml, htmlBuilder);
            }

            return new HtmlString(htmlBuilder.ToString());
        }

        private static void AppendHtml(IHtmlContent itemHtml, StringBuilder htmlBuilder)
        {
            StringWriter writer = new StringWriter();
            itemHtml.WriteTo(writer, HtmlEncoder.Default);
            htmlBuilder.Append(writer);
        }

        /// <summary>
        /// Used to manually generate the hidden &lt;input /&gt;. To be used in conjunction with EditorForMany(), when "false" was passed for includeIndexField. 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="html"></param>
        /// <param name="indexResolverExpression">An expression which points to the property on the model which holds the GUID index (optional, but required to make Validation* methods to work on post-back)</param>
        /// <returns>Generated HTML for hidden &lt;input /&gt;</returns>
        public static IHtmlContent EditorForManyIndexField<TModel>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, string>> indexResolverExpression = null)
        {
            var htmlPrefix = html.ViewData.TemplateInfo.HtmlFieldPrefix;
            var first = htmlPrefix.LastIndexOf('[');
            var last = htmlPrefix.IndexOf(']', first + 1);

            if (first == -1 || last == -1)
            {
                throw new InvalidOperationException(
                    "EditorForManyIndexField called when not in a EditorForMany context");
            }

            var htmlFieldNameWithPrefix = htmlPrefix.Substring(0, first);
            var guid = htmlPrefix.Substring(first + 1, last - first - 1);

            return _EditorForManyIndexField(htmlFieldNameWithPrefix, guid, indexResolverExpression);
        }

        private static IHtmlContent _EditorForManyIndexField<TModel>(string htmlFieldNameWithPrefix, string guid,
            Expression<Func<TModel, string>> indexResolverExpression)
        {
            var htmlBuilder = new StringBuilder();
            htmlBuilder.AppendFormat(@"<input type=""hidden"" name=""{0}.Index"" value=""{1}"" />",
                htmlFieldNameWithPrefix, guid);

            if (indexResolverExpression != null)
            {
                htmlBuilder.AppendFormat(@"<input type=""hidden"" name=""{0}[{1}].{2}"" value=""{1}"" />",
                    htmlFieldNameWithPrefix, guid, ExpressionHelper.GetExpressionText(indexResolverExpression));
            }

            return new HtmlString(htmlBuilder.ToString());
        }
    }
}