using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_createdTime_to_answers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "SimpleAnswers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SimpleAnswers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAt",
                table: "SimpleAnswers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "CorrectSequenceAnswers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CorrectSequenceAnswers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAt",
                table: "CorrectSequenceAnswers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "SimpleAnswers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SimpleAnswers");

            migrationBuilder.DropColumn(
                name: "EditedAt",
                table: "SimpleAnswers");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "CorrectSequenceAnswers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CorrectSequenceAnswers");

            migrationBuilder.DropColumn(
                name: "EditedAt",
                table: "CorrectSequenceAnswers");
        }
    }
}
