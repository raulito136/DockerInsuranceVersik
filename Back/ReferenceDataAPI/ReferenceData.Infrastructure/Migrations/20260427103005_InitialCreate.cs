using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReferenceData.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoverageTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ClaimStatuses",
                columns: new[] { "Id", "Code", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "SUBMITTED", "Claim has been filed and is awaiting review.", true, "Submitted" },
                    { 2, "UNDER_REVIEW", "A reviewer is actively assessing the claim.", true, "Under Review" },
                    { 3, "APPROVED", "The claim has been accepted; payment will be processed.", true, "Approved" },
                    { 4, "REJECTED", "The claim has been denied; no payment will be made.", true, "Rejected" },
                    { 5, "PAID", "Payment has been issued to the policy holder.", true, "Paid" }
                });

            migrationBuilder.InsertData(
                table: "CoverageTypes",
                columns: new[] { "Id", "Code", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "FULL", "All-risk coverage.", true, "Full" },
                    { 2, "PARTIAL", "Named-perils coverage.", true, "Partial" },
                    { 3, "THIRD_PARTY", "Covers legal liability to third parties.", true, "Third Party" },
                    { 4, "CATASTROPHIC", "Covers only extreme, large-scale loss events.", true, "Catastrophic" }
                });

            migrationBuilder.InsertData(
                table: "PolicyTypes",
                columns: new[] { "Id", "Code", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "AVIATION", "Insurance for aircraft, airlines, airports, and aviation-related liability.", true, "Aviation" },
                    { 2, "ENERGY", "Insurance for oil rigs, wind farms, refineries, pipelines, etc.", true, "Energy" },
                    { 3, "MARINE", "Insurance for ships, cargo, ports, and waterways.", true, "Marine" },
                    { 4, "CYBER", "Insurance against data breaches and ransomware attacks.", true, "Cyber" },
                    { 5, "LIABILITY", "General commercial liability insurance.", true, "Liability" }
                });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "NSW", true, "New South Wales" },
                    { 2, "VIC", true, "Victoria" },
                    { 3, "QLD", true, "Queensland" },
                    { 4, "WA", true, "Western Australia" },
                    { 5, "SA", true, "South Australia" },
                    { 6, "TAS", true, "Tasmania" },
                    { 7, "ACT", true, "Australian Capital Territory" },
                    { 8, "NT", true, "Northern Territory" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimStatuses_Code",
                table: "ClaimStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoverageTypes_Code",
                table: "CoverageTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyTypes_Code",
                table: "PolicyTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Code",
                table: "Regions",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimStatuses");

            migrationBuilder.DropTable(
                name: "CoverageTypes");

            migrationBuilder.DropTable(
                name: "PolicyTypes");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
