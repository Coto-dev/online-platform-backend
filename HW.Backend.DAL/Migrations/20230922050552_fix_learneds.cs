using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fix_learneds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Students_StudentId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_StudentId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Chapters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "Chapters",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_StudentId",
                table: "Chapters",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Students_StudentId",
                table: "Chapters",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
