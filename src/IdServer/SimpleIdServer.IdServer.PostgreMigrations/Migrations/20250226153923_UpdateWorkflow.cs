using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
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
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationWorkflow",
                table: "Acrs",
                type: "text",
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
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationMethodReferences",
                table: "Acrs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
