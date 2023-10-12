using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_reviewed_detailed_tests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewedDetailedTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DetailedAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewedDetailedTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewedDetailedTests_Teachers_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewedDetailedTests_UserAnswers_DetailedAnswerId",
                        column: x => x.DetailedAnswerId,
                        principalTable: "UserAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewedDetailedTests_DetailedAnswerId",
                table: "ReviewedDetailedTests",
                column: "DetailedAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewedDetailedTests_ReviewedById",
                table: "ReviewedDetailedTests",
                column: "ReviewedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewedDetailedTests");
        }
    }
}
