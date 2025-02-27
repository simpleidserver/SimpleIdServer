using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
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
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationWorkflow",
                table: "Acrs",
                type: "nvarchar(max)",
                nullable: true);
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
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationMethodReferences",
                table: "Acrs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
