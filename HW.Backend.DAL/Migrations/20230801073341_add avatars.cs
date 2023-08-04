using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Backend.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addavatars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarId",
                table: "Modules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarId",
                table: "EducationalPrograms",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "EducationalPrograms");
        }
    }
}
