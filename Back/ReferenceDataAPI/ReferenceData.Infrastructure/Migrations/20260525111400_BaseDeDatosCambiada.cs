using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReferenceData.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BaseDeDatosCambiada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reference-data");

            migrationBuilder.RenameTable(
                name: "Regions",
                newName: "Regions",
                newSchema: "reference-data");

            migrationBuilder.RenameTable(
                name: "PolicyTypes",
                newName: "PolicyTypes",
                newSchema: "reference-data");

            migrationBuilder.RenameTable(
                name: "CoverageTypes",
                newName: "CoverageTypes",
                newSchema: "reference-data");

            migrationBuilder.RenameTable(
                name: "ClaimStatuses",
                newName: "ClaimStatuses",
                newSchema: "reference-data");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Regions",
                schema: "reference-data",
                newName: "Regions");

            migrationBuilder.RenameTable(
                name: "PolicyTypes",
                schema: "reference-data",
                newName: "PolicyTypes");

            migrationBuilder.RenameTable(
                name: "CoverageTypes",
                schema: "reference-data",
                newName: "CoverageTypes");

            migrationBuilder.RenameTable(
                name: "ClaimStatuses",
                schema: "reference-data",
                newName: "ClaimStatuses");
        }
    }
}
