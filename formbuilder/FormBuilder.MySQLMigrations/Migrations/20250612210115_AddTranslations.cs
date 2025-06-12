using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.MySQLMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessageTranslations",
                table: "Forms",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SuccessMessageTranslations",
                table: "Forms",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessageTranslations",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "SuccessMessageTranslations",
                table: "Forms");
        }
    }
}
