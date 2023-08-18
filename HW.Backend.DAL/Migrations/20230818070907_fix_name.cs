using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fix_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAnswered",
                table: "UserAnswerTests",
                newName: "AnsweredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AnsweredAt",
                table: "UserAnswerTests",
                newName: "IsAnswered");
        }
    }
}
