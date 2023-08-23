using System;
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
                    AvatarId = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TimeDuration = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalPrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    AvatarId = table.Column<string>(type: "text", nullable: true),
                    TimeDuration = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModuleVisibility = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    StartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxStudents = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserBackends",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBackends", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationalProgramModule",
                columns: table => new
                {
                    ModulesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalProgramModule", x => new { x.ModulesId, x.ProgramsId });
                    table.ForeignKey(
                        name: "FK_EducationalProgramModule_EducationalPrograms_ProgramsId",
                        column: x => x.ProgramsId,
                        principalTable: "EducationalPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationalProgramModule_Modules_ModulesId",
                        column: x => x.ModulesId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecommendedModules",
                columns: table => new
                {
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecommendedModulesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendedModules", x => new { x.ModuleId, x.RecommendedModulesId });
                    table.ForeignKey(
                        name: "FK_RecommendedModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecommendedModules_Modules_RecommendedModulesId",
                        column: x => x.RecommendedModulesId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubModuleType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
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
                    IsTeacherComment = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleComments_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleComments_UserBackends_UserId",
                        column: x => x.UserId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_UserBackends_Id",
                        column: x => x.Id,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_UserBackends_Id",
                        column: x => x.Id,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chapters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<string>(type: "text", nullable: true),
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
                        name: "FK_Chapters_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Chapters_SubModules_SubModuleId",
                        column: x => x.SubModuleId,
                        principalTable: "SubModules",
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
                        name: "FK_UserActions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
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
                        name: "FK_UserModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModules_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationalProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramVisibilityType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPrograms_EducationalPrograms_EducationalProgramId",
                        column: x => x.EducationalProgramId,
                        principalTable: "EducationalPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrograms_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationalProgramTeacher",
                columns: table => new
                {
                    CreatedProgramsId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalProgramTeacher", x => new { x.CreatedProgramsId, x.CreatorsId });
                    table.ForeignKey(
                        name: "FK_EducationalProgramTeacher_EducationalPrograms_CreatedProgra~",
                        column: x => x.CreatedProgramsId,
                        principalTable: "EducationalPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationalProgramTeacher_Teachers_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_ModuleTeacher_Modules_CreatedModulesId",
                        column: x => x.CreatedModulesId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleTeacher_Teachers_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "Teachers",
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
                        name: "FK_ModuleTeacher1_Modules_ControlledModulesId",
                        column: x => x.ControlledModulesId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleTeacher1_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
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
                    Comment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterComments_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChapterComments_UserBackends_UserId",
                        column: x => x.UserId,
                        principalTable: "UserBackends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Learned",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    LearnDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                        name: "FK_Learned_Students_LearnedById",
                        column: x => x.LearnedById,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Order = table.Column<string>(type: "text", nullable: true),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    Files = table.Column<List<string>>(type: "text[]", nullable: true),
                    TestType = table.Column<int>(type: "integer", nullable: false),
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
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswerTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswerTests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnswerTests_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
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

            migrationBuilder.CreateIndex(
                name: "IX_ChapterComments_ChapterId",
                table: "ChapterComments",
                column: "ChapterId");

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
                name: "IX_EducationalProgramModule_ProgramsId",
                table: "EducationalProgramModule",
                column: "ProgramsId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationalProgramTeacher_CreatorsId",
                table: "EducationalProgramTeacher",
                column: "CreatorsId");

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
                name: "IX_ModuleComments_UserId",
                table: "ModuleComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleTeacher_CreatorsId",
                table: "ModuleTeacher",
                column: "CreatorsId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleTeacher1_TeachersId",
                table: "ModuleTeacher1",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendedModules_RecommendedModulesId",
                table: "RecommendedModules",
                column: "RecommendedModulesId");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserPrograms_EducationalProgramId",
                table: "UserPrograms",
                column: "EducationalProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrograms_StudentId",
                table: "UserPrograms",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterComments");

            migrationBuilder.DropTable(
                name: "EducationalProgramModule");

            migrationBuilder.DropTable(
                name: "EducationalProgramTeacher");

            migrationBuilder.DropTable(
                name: "Learned");

            migrationBuilder.DropTable(
                name: "ModuleComments");

            migrationBuilder.DropTable(
                name: "ModuleTeacher");

            migrationBuilder.DropTable(
                name: "ModuleTeacher1");

            migrationBuilder.DropTable(
                name: "RecommendedModules");

            migrationBuilder.DropTable(
                name: "UserActions");

            migrationBuilder.DropTable(
                name: "UserAnswers");

            migrationBuilder.DropTable(
                name: "UserModules");

            migrationBuilder.DropTable(
                name: "UserPrograms");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "CorrectSequenceAnswers");

            migrationBuilder.DropTable(
                name: "SimpleAnswers");

            migrationBuilder.DropTable(
                name: "UserAnswerTests");

            migrationBuilder.DropTable(
                name: "EducationalPrograms");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Chapters");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "SubModules");

            migrationBuilder.DropTable(
                name: "UserBackends");

            migrationBuilder.DropTable(
                name: "Modules");
        }
    }
}
