using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileContactAndBadgeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ProfileTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegreeBadge",
                table: "ProfileTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceBadge",
                table: "ProfileTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ProfileTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortraitAlt",
                table: "ProfileTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleBadge",
                table: "ProfileTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "ProfileTranslations");

            migrationBuilder.DropColumn(
                name: "DegreeBadge",
                table: "ProfileTranslations");

            migrationBuilder.DropColumn(
                name: "ExperienceBadge",
                table: "ProfileTranslations");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "ProfileTranslations");

            migrationBuilder.DropColumn(
                name: "PortraitAlt",
                table: "ProfileTranslations");

            migrationBuilder.DropColumn(
                name: "RoleBadge",
                table: "ProfileTranslations");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Profiles");
        }
    }
}
