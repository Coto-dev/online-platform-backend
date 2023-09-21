using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_chapter_blocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<Guid>>(
                name: "OrderedBlocks",
                table: "Chapters",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChapterBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Files = table.Column<List<string>>(type: "text[]", nullable: true),
                    ChapterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterBlocks_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterBlocks_ChapterId",
                table: "ChapterBlocks",
                column: "ChapterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterBlocks");

            migrationBuilder.DropColumn(
                name: "OrderedBlocks",
                table: "Chapters");
        }
    }
}
