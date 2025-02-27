using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.MySQLMigrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Steps",
                table: "RegistrationWorkflows");

            migrationBuilder.DropColumn(
                name: "AuthenticationMethodReferences",
                table: "Acrs");

            migrationBuilder.AddColumn<string>(
                name: "WorkflowId",
                table: "RegistrationWorkflows",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationWorkflow",
                table: "Acrs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "RegistrationWorkflows");

            migrationBuilder.DropColumn(
                name: "AuthenticationWorkflow",
                table: "Acrs");

            migrationBuilder.AddColumn<string>(
                name: "Steps",
                table: "RegistrationWorkflows",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationMethodReferences",
                table: "Acrs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
