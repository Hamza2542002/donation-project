using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace donation_project.Migrations
{
    /// <inheritdoc />
    public partial class updateOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAuthenticated",
                table: "OTPs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAuthenticated",
                table: "OTPs");
        }
    }
}
