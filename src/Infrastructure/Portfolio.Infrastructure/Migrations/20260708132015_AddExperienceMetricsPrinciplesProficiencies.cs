using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExperienceMetricsPrinciplesProficiencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImpactMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpactMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Principles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Principles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProficiencyGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProficiencyGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImpactMetricTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImpactMetricId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpactMetricTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpactMetricTranslations_ImpactMetrics_ImpactMetricId",
                        column: x => x.ImpactMetricId,
                        principalTable: "ImpactMetrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrincipleTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrincipleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipleTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrincipleTranslations_Principles_PrincipleId",
                        column: x => x.PrincipleId,
                        principalTable: "Principles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProficiencyGroupTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProficiencyGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Items = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProficiencyGroupTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProficiencyGroupTranslations_ProficiencyGroups_ProficiencyGroupId",
                        column: x => x.ProficiencyGroupId,
                        principalTable: "ProficiencyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImpactMetricTranslations_ImpactMetricId",
                table: "ImpactMetricTranslations",
                column: "ImpactMetricId");

            migrationBuilder.CreateIndex(
                name: "IX_PrincipleTranslations_PrincipleId",
                table: "PrincipleTranslations",
                column: "PrincipleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProficiencyGroupTranslations_ProficiencyGroupId",
                table: "ProficiencyGroupTranslations",
                column: "ProficiencyGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImpactMetricTranslations");

            migrationBuilder.DropTable(
                name: "PrincipleTranslations");

            migrationBuilder.DropTable(
                name: "ProficiencyGroupTranslations");

            migrationBuilder.DropTable(
                name: "ImpactMetrics");

            migrationBuilder.DropTable(
                name: "Principles");

            migrationBuilder.DropTable(
                name: "ProficiencyGroups");
        }
    }
}
