﻿// <auto-generated />
using System;
using FormBuilder.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FormBuilder.SqliteMigrations.Migrations
{
    [DbContext(typeof(FormBuilderDbContext))]
    [Migration("20250226151730_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("FormBuilder.Models.FormRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<bool>("ActAsStep")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .HasColumnType("TEXT");

                    b.Property<string>("CorrelationId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Elements")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Realm")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdateDateTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("VersionNumber")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("FormBuilder.Models.FormStyle", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FormId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FormId");

                    b.ToTable("FormStyle");
                });

            modelBuilder.Entity("FormBuilder.Models.WorkflowLink", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ActionParameter")
                        .HasColumnType("TEXT");

                    b.Property<string>("ActionType")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsMainLink")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceStepId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TargetStepId")
                        .HasColumnType("TEXT");

                    b.Property<string>("WorkflowRecordId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowRecordId");

                    b.ToTable("WorkflowLink");
                });

            modelBuilder.Entity("FormBuilder.Models.WorkflowRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Realm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdateDateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Workflows");
                });

            modelBuilder.Entity("FormBuilder.Models.WorkflowStep", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("FormRecordCorrelationId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WorkflowRecordId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowRecordId");

                    b.ToTable("WorkflowStep");
                });

            modelBuilder.Entity("FormBuilder.Models.FormStyle", b =>
                {
                    b.HasOne("FormBuilder.Models.FormRecord", null)
                        .WithMany("AvailableStyles")
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FormBuilder.Models.WorkflowLink", b =>
                {
                    b.HasOne("FormBuilder.Models.WorkflowRecord", null)
                        .WithMany("Links")
                        .HasForeignKey("WorkflowRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FormBuilder.Models.WorkflowStep", b =>
                {
                    b.HasOne("FormBuilder.Models.WorkflowRecord", null)
                        .WithMany("Steps")
                        .HasForeignKey("WorkflowRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FormBuilder.Models.FormRecord", b =>
                {
                    b.Navigation("AvailableStyles");
                });

            modelBuilder.Entity("FormBuilder.Models.WorkflowRecord", b =>
                {
                    b.Navigation("Links");

                    b.Navigation("Steps");
                });
#pragma warning restore 612, 618
        }
    }
}
