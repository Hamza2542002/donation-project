using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace donation_project.Migrations
{
    /// <inheritdoc />
    public partial class updateOTPmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs");

            migrationBuilder.CreateIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            // Recreate the unique index
            migrationBuilder.CreateIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs",
                column: "UserId",
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs");
        }
    }
}
