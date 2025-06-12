using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.PostgreMigrations.Migrations
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
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SuccessMessageTranslations",
                table: "Forms",
                type: "text",
                nullable: false,
                defaultValue: "");
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
