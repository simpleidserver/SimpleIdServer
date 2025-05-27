using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.SqliteMigrations.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "WorkflowLink");

            migrationBuilder.AddColumn<string>(
                name: "Targets",
                table: "WorkflowLink",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Targets",
                table: "WorkflowLink");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "WorkflowLink",
                type: "TEXT",
                nullable: true);
        }
    }
}
