using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CambioBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "claims");

            migrationBuilder.RenameTable(
                name: "Claims",
                newName: "Claims",
                newSchema: "claims");

            migrationBuilder.RenameTable(
                name: "ClaimComments",
                newName: "ClaimComments",
                newSchema: "claims");

            migrationBuilder.RenameTable(
                name: "ClaimAudits",
                newName: "ClaimAudits",
                newSchema: "claims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Claims",
                schema: "claims",
                newName: "Claims");

            migrationBuilder.RenameTable(
                name: "ClaimComments",
                schema: "claims",
                newName: "ClaimComments");

            migrationBuilder.RenameTable(
                name: "ClaimAudits",
                schema: "claims",
                newName: "ClaimAudits");
        }
    }
}
