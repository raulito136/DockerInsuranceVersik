using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliciesService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CambioBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "policy");

            migrationBuilder.RenameTable(
                name: "PolicyHolders",
                newName: "PolicyHolders",
                newSchema: "policy");

            migrationBuilder.RenameTable(
                name: "Policies",
                newName: "Policies",
                newSchema: "policy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PolicyHolders",
                schema: "policy",
                newName: "PolicyHolders");

            migrationBuilder.RenameTable(
                name: "Policies",
                schema: "policy",
                newName: "Policies");
        }
    }
}
