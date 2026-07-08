using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTimelineEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimelineEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimelineEntryTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimelineEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineEntryTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimelineEntryTranslations_TimelineEntries_TimelineEntryId",
                        column: x => x.TimelineEntryId,
                        principalTable: "TimelineEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimelineEntryTranslations_TimelineEntryId",
                table: "TimelineEntryTranslations",
                column: "TimelineEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimelineEntryTranslations");

            migrationBuilder.DropTable(
                name: "TimelineEntries");
        }
    }
}
