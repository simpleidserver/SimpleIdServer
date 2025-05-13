using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Scopes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Groups",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ApiResources",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MigrationExecutions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MigrationExecutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MigrationExecutionHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    StartDatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    NbRecords = table.Column<int>(type: "integer", nullable: false),
                    Errors = table.Column<string>(type: "text", nullable: false),
                    MigrationExecutionId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MigrationExecutionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MigrationExecutionHistory_MigrationExecutions_MigrationExec~",
                        column: x => x.MigrationExecutionId,
                        principalTable: "MigrationExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MigrationExecutionHistory_MigrationExecutionId",
                table: "MigrationExecutionHistory",
                column: "MigrationExecutionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MigrationExecutionHistory");

            migrationBuilder.DropTable(
                name: "MigrationExecutions");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Scopes");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "ApiResources");
        }
    }
}
