﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RundownEdu.Models;

namespace RundownEdu.Migrations
{
    [DbContext(typeof(RundownEduDBContext))]
    partial class RundownEduDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RundownEdu.Models.Rundown", b =>
                {
                    b.Property<int>("RundownId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<DateTime>("EndTime");

                    b.Property<int>("ShowId");

                    b.Property<DateTime>("StartTime");

                    b.Property<string>("Title");

                    b.HasKey("RundownId");

                    b.HasIndex("ShowId");

                    b.ToTable("Rundowns");
                });

            modelBuilder.Entity("RundownEdu.Models.Segment", b =>
                {
                    b.Property<int>("SegmentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActualReadTime");

                    b.Property<string>("EstimatedReadTime");

                    b.Property<string>("Reader");

                    b.Property<int>("StoryId");

                    b.Property<int>("Type");

                    b.HasKey("SegmentId");

                    b.HasIndex("StoryId");

                    b.ToTable("Segments");
                });

            modelBuilder.Entity("RundownEdu.Models.Show", b =>
                {
                    b.Property<int>("ShowId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<string>("Title");

                    b.HasKey("ShowId");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("RundownEdu.Models.Story", b =>
                {
                    b.Property<int>("StoryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RundownId");

                    b.Property<string>("Title");

                    b.HasKey("StoryId");

                    b.HasIndex("RundownId");

                    b.ToTable("Stories");
                });

            modelBuilder.Entity("RundownEdu.Models.Rundown", b =>
                {
                    b.HasOne("RundownEdu.Models.Show", "Show")
                        .WithMany("Rundowns")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RundownEdu.Models.Segment", b =>
                {
                    b.HasOne("RundownEdu.Models.Story", "Story")
                        .WithMany("Segments")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RundownEdu.Models.Story", b =>
                {
                    b.HasOne("RundownEdu.Models.Rundown", "Rundown")
                        .WithMany("Stories")
                        .HasForeignKey("RundownId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}