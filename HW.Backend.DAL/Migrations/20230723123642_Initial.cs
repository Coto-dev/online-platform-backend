﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducationalPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalPrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserBackends",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBackends", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationalProgramStudent",
                columns: table => new
                {
                    EducationalProgramsId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalProgramStudent", x => new { x.EducationalProgramsId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_EducationalProgramStudent_EducationalPrograms_EducationalPr~",
                        column: x => x.EducationalProgramsId,
                        principalTable: "EducationalPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationalProgramStudent_UserBackends_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Actions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActions_UserBackends_StudentId",
                        column: x => x.StudentId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChapterComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsTeacherComment = table.Column<bool>(type: "boolean", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterComments_UserBackends_StudentId",
                        column: x => x.StudentId,
                        principalTable: "UserBackends",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChapterComments_UserBackends_UserId",
                        column: x => x.UserId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    SubModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Files = table.Column<List<string>>(type: "text[]", nullable: true),
                    ChapterType = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chapters_UserBackends_StudentId",
                        column: x => x.StudentId,
                        principalTable: "UserBackends",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Learned",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Learned_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Learned_UserBackends_LearnedById",
                        column: x => x.LearnedById,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    Files = table.Column<List<string>>(type: "text[]", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorrectSequenceAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerContent = table.Column<string>(type: "text", nullable: false),
                    RightOrder = table.Column<int>(type: "integer", nullable: false),
                    CorrectSequenceTestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectSequenceAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorrectSequenceAnswers_Tests_CorrectSequenceTestId",
                        column: x => x.CorrectSequenceTestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimpleAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerContent = table.Column<string>(type: "text", nullable: false),
                    IsRight = table.Column<bool>(type: "boolean", nullable: false),
                    SimpleAnswerTestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimpleAnswers_Tests_SimpleAnswerTestId",
                        column: x => x.SimpleAnswerTestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswerTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumberOfAttempt = table.Column<int>(type: "integer", nullable: false),
                    IsAnswered = table.Column<bool>(type: "boolean", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswerTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswerTests_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnswerTests_UserBackends_StudentId",
                        column: x => x.StudentId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserAnswerTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    CorrectSequenceAnswerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: true),
                    AnswerContent = table.Column<string>(type: "text", nullable: true),
                    Accuracy = table.Column<int>(type: "integer", nullable: true),
                    Files = table.Column<List<string>>(type: "text[]", nullable: true),
                    SimpleAnswerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswers_CorrectSequenceAnswers_CorrectSequenceAnswerId",
                        column: x => x.CorrectSequenceAnswerId,
                        principalTable: "CorrectSequenceAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnswers_SimpleAnswers_SimpleAnswerId",
                        column: x => x.SimpleAnswerId,
                        principalTable: "SimpleAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnswers_UserAnswerTests_UserAnswerTestId",
                        column: x => x.UserAnswerTestId,
                        principalTable: "UserAnswerTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsTeacherComment = table.Column<bool>(type: "boolean", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleComments_UserBackends_StudentId",
                        column: x => x.StudentId,
                        principalTable: "UserBackends",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModuleComments_UserBackends_UserId",
                        column: x => x.UserId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleInEducationalPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationalProgramId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleInEducationalPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleInEducationalPrograms_EducationalPrograms_Educational~",
                        column: x => x.EducationalProgramId,
                        principalTable: "EducationalPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleTeachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModuleVisibility = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    ModuleInEducationalProgramId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxStudents = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleTeachers_ModuleInEducationalPrograms_ModuleInEducatio~",
                        column: x => x.ModuleInEducationalProgramId,
                        principalTable: "ModuleInEducationalPrograms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ModuleTeacher",
                columns: table => new
                {
                    CreatedModulesId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleTeacher", x => new { x.CreatedModulesId, x.CreatorsId });
                    table.ForeignKey(
                        name: "FK_ModuleTeacher_ModuleTeachers_CreatedModulesId",
                        column: x => x.CreatedModulesId,
                        principalTable: "ModuleTeachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleTeacher_UserBackends_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleTeacher1",
                columns: table => new
                {
                    ControlledModulesId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeachersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleTeacher1", x => new { x.ControlledModulesId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_ModuleTeacher1_ModuleTeachers_ControlledModulesId",
                        column: x => x.ControlledModulesId,
                        principalTable: "ModuleTeachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleTeacher1_UserBackends_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubModuleType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubModules_ModuleTeachers_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "ModuleTeachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserModules_ModuleTeachers_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "ModuleTeachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModules_UserBackends_StudentId",
                        column: x => x.StudentId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterComments_ChapterId",
                table: "ChapterComments",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterComments_StudentId",
                table: "ChapterComments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterComments_UserId",
                table: "ChapterComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_StudentId",
                table: "Chapters",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_SubModuleId",
                table: "Chapters",
                column: "SubModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectSequenceAnswers_CorrectSequenceTestId",
                table: "CorrectSequenceAnswers",
                column: "CorrectSequenceTestId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationalProgramStudent_StudentsId",
                table: "EducationalProgramStudent",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Learned_ChapterId",
                table: "Learned",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_Learned_LearnedById",
                table: "Learned",
                column: "LearnedById");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleComments_ModuleId",
                table: "ModuleComments",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleComments_StudentId",
                table: "ModuleComments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleComments_UserId",
                table: "ModuleComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleInEducationalPrograms_EducationalProgramId",
                table: "ModuleInEducationalPrograms",
                column: "EducationalProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleInEducationalPrograms_ModuleId",
                table: "ModuleInEducationalPrograms",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleTeacher_CreatorsId",
                table: "ModuleTeacher",
                column: "CreatorsId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleTeacher1_TeachersId",
                table: "ModuleTeacher1",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleTeachers_ModuleInEducationalProgramId",
                table: "ModuleTeachers",
                column: "ModuleInEducationalProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SimpleAnswers_SimpleAnswerTestId",
                table: "SimpleAnswers",
                column: "SimpleAnswerTestId");

            migrationBuilder.CreateIndex(
                name: "IX_SubModules_ModuleId",
                table: "SubModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_ChapterId",
                table: "Tests",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActions_StudentId",
                table: "UserActions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_CorrectSequenceAnswerId",
                table: "UserAnswers",
                column: "CorrectSequenceAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_SimpleAnswerId",
                table: "UserAnswers",
                column: "SimpleAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_UserAnswerTestId",
                table: "UserAnswers",
                column: "UserAnswerTestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswerTests_StudentId",
                table: "UserAnswerTests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswerTests_TestId",
                table: "UserAnswerTests",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModules_ModuleId",
                table: "UserModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModules_StudentId",
                table: "UserModules",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterComments_Chapters_ChapterId",
                table: "ChapterComments",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_SubModules_SubModuleId",
                table: "Chapters",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleComments_ModuleTeachers_ModuleId",
                table: "ModuleComments",
                column: "ModuleId",
                principalTable: "ModuleTeachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleInEducationalPrograms_ModuleTeachers_ModuleId",
                table: "ModuleInEducationalPrograms",
                column: "ModuleId",
                principalTable: "ModuleTeachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleInEducationalPrograms_EducationalPrograms_Educational~",
                table: "ModuleInEducationalPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleInEducationalPrograms_ModuleTeachers_ModuleId",
                table: "ModuleInEducationalPrograms");

            migrationBuilder.DropTable(
                name: "ChapterComments");

            migrationBuilder.DropTable(
                name: "EducationalProgramStudent");

            migrationBuilder.DropTable(
                name: "Learned");

            migrationBuilder.DropTable(
                name: "ModuleComments");

            migrationBuilder.DropTable(
                name: "ModuleTeacher");

            migrationBuilder.DropTable(
                name: "ModuleTeacher1");

            migrationBuilder.DropTable(
                name: "UserActions");

            migrationBuilder.DropTable(
                name: "UserAnswers");

            migrationBuilder.DropTable(
                name: "UserModules");

            migrationBuilder.DropTable(
                name: "CorrectSequenceAnswers");

            migrationBuilder.DropTable(
                name: "SimpleAnswers");

            migrationBuilder.DropTable(
                name: "UserAnswerTests");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "SubModules");

            migrationBuilder.DropTable(
                name: "UserBackends");

            migrationBuilder.DropTable(
                name: "EducationalPrograms");

            migrationBuilder.DropTable(
                name: "ModuleTeachers");

            migrationBuilder.DropTable(
                name: "ModuleInEducationalPrograms");
        }
    }
}
