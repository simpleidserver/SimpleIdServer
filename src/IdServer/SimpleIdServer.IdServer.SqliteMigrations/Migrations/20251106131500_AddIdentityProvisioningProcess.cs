using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace SimpleIdServer.IdServer.SqliteMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityProvisioningProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityProvisioningProcesses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartExportDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndExportDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartImportDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndImportDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NbExtractedPages = table.Column<int>(type: "INTEGER", nullable: false),
                    NbExtractedUsers = table.Column<int>(type: "INTEGER", nullable: false),
                    NbExtractedGroups = table.Column<int>(type: "INTEGER", nullable: false),
                    NbFilteredRepresentations = table.Column<int>(type: "INTEGER", nullable: false),
                    NbImportedPages = table.Column<int>(type: "INTEGER", nullable: false),
                    NbImportedGroups = table.Column<int>(type: "INTEGER", nullable: false),
                    NbImportedUsers = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPageToExtract = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPageToImport = table.Column<int>(type: "INTEGER", nullable: false),
                    IdentityProvisioningId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningProcesses_IdentityProvisioningLst_IdentityProvisioningId",
                        column: x => x.IdentityProvisioningId,
                        principalTable: "IdentityProvisioningLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningProcesses_IdentityProvisioningId",
                table: "IdentityProvisioningProcesses",
                column: "IdentityProvisioningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityProvisioningProcesses");
        }
    }
}
