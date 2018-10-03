﻿// <auto-generated />
using System;
using CoreTaskManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoreTaskManager.Migrations
{
    [DbContext(typeof(CoreTaskManagerContext))]
    [Migration("20181003120524_ModifyTaskModel")]
    partial class ModifyTaskModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoreTaskManager.Model.Participant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrentProgress");

                    b.Property<int>("ProgressId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("CoreTaskManager.Model.Progress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<string>("Genre")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Image");

                    b.Property<int>("NumberOfItems");

                    b.Property<DateTime>("RegisteredDateTime");

                    b.Property<string>("SlackAppUrl");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("Progress");
                });

            modelBuilder.Entity("CoreTaskManager.Model.TaskModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ProgressId");

                    b.Property<string>("TaskName")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.HasKey("Id");

                    b.ToTable("Task");
                });
#pragma warning restore 612, 618
        }
    }
}
