﻿// <auto-generated />
using System;
using System.Collections.Generic;
using HW.Backend.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    [DbContext(typeof(BackendDbContext))]
    [Migration("20230723123642_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EducationalProgramStudent", b =>
                {
                    b.Property<Guid>("EducationalProgramsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StudentsId")
                        .HasColumnType("uuid");

                    b.HasKey("EducationalProgramsId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("EducationalProgramStudent");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Chapter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ChapterType")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<List<string>>("Files")
                        .HasColumnType("text[]");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SubModuleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.HasIndex("SubModuleId");

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.ChapterComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChapterId")
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsTeacherComment")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ChapterId");

                    b.HasIndex("StudentId");

                    b.HasIndex("UserId");

                    b.ToTable("ChapterComments");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.CorrectSequenceAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AnswerContent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("CorrectSequenceTestId")
                        .HasColumnType("uuid");

                    b.Property<int>("RightOrder")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CorrectSequenceTestId");

                    b.ToTable("CorrectSequenceAnswers");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.EducationalProgram", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ArchivedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("EditedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("EducationalPrograms");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Learned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChapterId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("LearnDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("LearnedById")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ChapterId");

                    b.HasIndex("LearnedById");

                    b.ToTable("Learned");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Module", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ArchivedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("EditedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ModuleInEducationalProgramId")
                        .HasColumnType("uuid");

                    b.Property<int>("ModuleVisibility")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ModuleInEducationalProgramId");

                    b.ToTable("ModuleTeachers", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("Module");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.ModuleComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("EditedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsTeacherComment")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ModuleId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ModuleId");

                    b.HasIndex("StudentId");

                    b.HasIndex("UserId");

                    b.ToTable("ModuleComments");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.ModuleInEducationalProgram", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("EducationalProgramId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ModuleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("EducationalProgramId");

                    b.HasIndex("ModuleId");

                    b.ToTable("ModuleInEducationalPrograms");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SimpleAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AnswerContent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsRight")
                        .HasColumnType("boolean");

                    b.Property<Guid>("SimpleAnswerTestId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SimpleAnswerTestId");

                    b.ToTable("SimpleAnswers");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.StudentModule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ModuleId")
                        .HasColumnType("uuid");

                    b.Property<int>("ModuleStatus")
                        .HasColumnType("integer");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ModuleId");

                    b.HasIndex("StudentId");

                    b.ToTable("UserModules");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SubModule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ModuleId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SubModuleType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ModuleId");

                    b.ToTable("SubModules");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Test", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChapterId")
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<List<string>>("Files")
                        .HasColumnType("text[]");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChapterId");

                    b.ToTable("Tests");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Test");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.UserAction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Actions")
                        .HasColumnType("integer");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("UserActions");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.UserAnswer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserAnswerTestId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserAnswerTestId");

                    b.ToTable("UserAnswers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("UserAnswer");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.UserAnswerTest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsAnswered")
                        .HasColumnType("boolean");

                    b.Property<int>("NumberOfAttempt")
                        .HasColumnType("integer");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TestId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.HasIndex("TestId");

                    b.ToTable("UserAnswerTests");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.UserBackend", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserBackends");

                    b.HasDiscriminator<string>("Discriminator").HasValue("UserBackend");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("ModuleTeacher", b =>
                {
                    b.Property<Guid>("CreatedModulesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CreatorsId")
                        .HasColumnType("uuid");

                    b.HasKey("CreatedModulesId", "CreatorsId");

                    b.HasIndex("CreatorsId");

                    b.ToTable("ModuleTeacher");
                });

            modelBuilder.Entity("ModuleTeacher1", b =>
                {
                    b.Property<Guid>("ControlledModulesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TeachersId")
                        .HasColumnType("uuid");

                    b.HasKey("ControlledModulesId", "TeachersId");

                    b.HasIndex("TeachersId");

                    b.ToTable("ModuleTeacher1");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.StreamingModule", b =>
                {
                    b.HasBaseType("HW.Backend.DAL.Data.Entities.Module");

                    b.Property<DateTime?>("ExpiredAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MaxStudents")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasDiscriminator().HasValue("StreamingModule");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.CorrectSequenceTest", b =>
                {
                    b.HasBaseType("HW.Backend.DAL.Data.Entities.Test");

                    b.HasDiscriminator().HasValue("CorrectSequenceTest");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SimpleAnswerTest", b =>
                {
                    b.HasBaseType("HW.Backend.DAL.Data.Entities.Test");

                    b.HasDiscriminator().HasValue("SimpleAnswerTest");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.CorrectSequenceUserAnswer", b =>
                {
                    b.HasBaseType("HW.Backend.DAL.Data.Entities.UserAnswer");

                    b.Property<Guid>("CorrectSequenceAnswerId")
                        .HasColumnType("uuid");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.HasIndex("CorrectSequenceAnswerId");

                    b.HasDiscriminator().HasValue("CorrectSequenceUserAnswer");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.DetailedAnswer", b =>
                {
                    b.HasBaseType("HW.Backend.DAL.Data.Entities.UserAnswer");

                    b.Property<int>("Accuracy")
                        .HasColumnType("integer");

                    b.Property<string>("AnswerContent")
                        .HasColumnType("text");

                    b.Property<List<string>>("Files")
                        .HasColumnType("text[]");

                    b.HasDiscriminator().HasValue("DetailedAnswer");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SimpleUserAnswer", b =>
                {
                    b.HasBaseType("HW.Backend.DAL.Data.Entities.UserAnswer");

                    b.Property<Guid>("SimpleAnswerId")
                        .HasColumnType("uuid");

                    b.HasIndex("SimpleAnswerId");

                    b.HasDiscriminator().HasValue("SimpleUserAnswer");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Student", b =>
                {
                    b.HasBaseType("HW.Backend.DAL.Data.Entities.UserBackend");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Teacher", b =>
                {
                    b.HasBaseType("HW.Backend.DAL.Data.Entities.UserBackend");

                    b.HasDiscriminator().HasValue("Teacher");
                });

            modelBuilder.Entity("EducationalProgramStudent", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.EducationalProgram", null)
                        .WithMany()
                        .HasForeignKey("EducationalProgramsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.Student", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Chapter", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Student", null)
                        .WithMany("LearnedChapters")
                        .HasForeignKey("StudentId");

                    b.HasOne("HW.Backend.DAL.Data.Entities.SubModule", "SubModule")
                        .WithMany("Chapters")
                        .HasForeignKey("SubModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubModule");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.ChapterComment", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Chapter", "Chapter")
                        .WithMany()
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.Student", null)
                        .WithMany("ChapterComments")
                        .HasForeignKey("StudentId");

                    b.HasOne("HW.Backend.DAL.Data.Entities.UserBackend", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chapter");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.CorrectSequenceAnswer", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.CorrectSequenceTest", "CorrectSequenceTest")
                        .WithMany("PossibleAnswers")
                        .HasForeignKey("CorrectSequenceTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CorrectSequenceTest");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Learned", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Chapter", "Chapter")
                        .WithMany()
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.UserBackend", "LearnedBy")
                        .WithMany()
                        .HasForeignKey("LearnedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chapter");

                    b.Navigation("LearnedBy");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Module", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.ModuleInEducationalProgram", null)
                        .WithMany("RequRequiredModules")
                        .HasForeignKey("ModuleInEducationalProgramId");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.ModuleComment", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Module", "Module")
                        .WithMany()
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.Student", null)
                        .WithMany("ModuleComments")
                        .HasForeignKey("StudentId");

                    b.HasOne("HW.Backend.DAL.Data.Entities.UserBackend", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.ModuleInEducationalProgram", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.EducationalProgram", "EducationalProgram")
                        .WithMany("AvailableModules")
                        .HasForeignKey("EducationalProgramId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.Module", "Module")
                        .WithMany()
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EducationalProgram");

                    b.Navigation("Module");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SimpleAnswer", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.SimpleAnswerTest", "SimpleAnswerTest")
                        .WithMany("PossibleAnswers")
                        .HasForeignKey("SimpleAnswerTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SimpleAnswerTest");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.StudentModule", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Module", "Module")
                        .WithMany("UserModules")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.Student", "Student")
                        .WithMany("Modules")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SubModule", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Module", "Module")
                        .WithMany("SubModules")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Test", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Chapter", "Chapter")
                        .WithMany("ChapterTests")
                        .HasForeignKey("ChapterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chapter");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.UserAction", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.UserAnswer", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.UserAnswerTest", "UserAnswerTest")
                        .WithMany("UserAnswers")
                        .HasForeignKey("UserAnswerTestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserAnswerTest");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.UserAnswerTest", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.Test", "Test")
                        .WithMany()
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");

                    b.Navigation("Test");
                });

            modelBuilder.Entity("ModuleTeacher", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Module", null)
                        .WithMany()
                        .HasForeignKey("CreatedModulesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.Teacher", null)
                        .WithMany()
                        .HasForeignKey("CreatorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ModuleTeacher1", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.Module", null)
                        .WithMany()
                        .HasForeignKey("ControlledModulesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HW.Backend.DAL.Data.Entities.Teacher", null)
                        .WithMany()
                        .HasForeignKey("TeachersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.CorrectSequenceUserAnswer", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.CorrectSequenceAnswer", "CorrectSequenceAnswer")
                        .WithMany()
                        .HasForeignKey("CorrectSequenceAnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CorrectSequenceAnswer");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SimpleUserAnswer", b =>
                {
                    b.HasOne("HW.Backend.DAL.Data.Entities.SimpleAnswer", "SimpleAnswer")
                        .WithMany()
                        .HasForeignKey("SimpleAnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SimpleAnswer");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Chapter", b =>
                {
                    b.Navigation("ChapterTests");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.EducationalProgram", b =>
                {
                    b.Navigation("AvailableModules");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Module", b =>
                {
                    b.Navigation("SubModules");

                    b.Navigation("UserModules");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.ModuleInEducationalProgram", b =>
                {
                    b.Navigation("RequRequiredModules");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SubModule", b =>
                {
                    b.Navigation("Chapters");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.UserAnswerTest", b =>
                {
                    b.Navigation("UserAnswers");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.CorrectSequenceTest", b =>
                {
                    b.Navigation("PossibleAnswers");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.SimpleAnswerTest", b =>
                {
                    b.Navigation("PossibleAnswers");
                });

            modelBuilder.Entity("HW.Backend.DAL.Data.Entities.Student", b =>
                {
                    b.Navigation("ChapterComments");

                    b.Navigation("LearnedChapters");

                    b.Navigation("ModuleComments");

                    b.Navigation("Modules");
                });
#pragma warning restore 612, 618
        }
    }
}
