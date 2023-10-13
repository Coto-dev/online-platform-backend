using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_status_to_user_answer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserAnswerTests",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserAnswerTests");
        }
    }
}
