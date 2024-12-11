using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace donation_project.Migrations
{
    /// <inheritdoc />
    public partial class addOTPModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the table
            migrationBuilder.CreateTable(
                name: "OTPs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Auto-increment ID
                    UserId = table.Column<string>(nullable: false), // Foreign key
                    OTP = table.Column<string>(maxLength: 6, nullable: true),
                    ExpiresOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPs", x => x.Id); // Primary key
                    table.ForeignKey(
                        name: "FK_OTPs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers", // Assuming User table is `AspNetUsers`
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Add a unique constraint to UserId
            migrationBuilder.CreateIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "OTPs");
        }
    }
}
