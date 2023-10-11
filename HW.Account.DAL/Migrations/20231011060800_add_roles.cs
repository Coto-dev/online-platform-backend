using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HW.Account.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "AspNetRoles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "AspNetRoles");
        }
    }
}
