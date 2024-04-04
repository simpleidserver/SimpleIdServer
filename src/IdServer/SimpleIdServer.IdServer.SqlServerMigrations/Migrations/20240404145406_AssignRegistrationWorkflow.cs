using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AssignRegistrationWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthenticationContextClassReferenceId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationWorkflowId",
                table: "Acrs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AuthenticationContextClassReferenceId",
                table: "Clients",
                column: "AuthenticationContextClassReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Acrs_RegistrationWorkflowId",
                table: "Acrs",
                column: "RegistrationWorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Acrs_RegistrationWorkflows_RegistrationWorkflowId",
                table: "Acrs",
                column: "RegistrationWorkflowId",
                principalTable: "RegistrationWorkflows",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Acrs_AuthenticationContextClassReferenceId",
                table: "Clients",
                column: "AuthenticationContextClassReferenceId",
                principalTable: "Acrs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Acrs_RegistrationWorkflows_RegistrationWorkflowId",
                table: "Acrs");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Acrs_AuthenticationContextClassReferenceId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_AuthenticationContextClassReferenceId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Acrs_RegistrationWorkflowId",
                table: "Acrs");

            migrationBuilder.DropColumn(
                name: "AuthenticationContextClassReferenceId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RegistrationWorkflowId",
                table: "Acrs");
        }
    }
}
