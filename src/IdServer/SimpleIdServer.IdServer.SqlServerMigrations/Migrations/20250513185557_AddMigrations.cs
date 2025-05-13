using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
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
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ApiResources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MigrationExecutions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Realm = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MigrationExecutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MigrationExecutionHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDatetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    NbRecords = table.Column<int>(type: "int", nullable: false),
                    Errors = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MigrationExecutionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MigrationExecutionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MigrationExecutionHistory_MigrationExecutions_MigrationExecutionId",
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
