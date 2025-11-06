using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartExportDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndExportDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartImportDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndImportDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NbExtractedPages = table.Column<int>(type: "int", nullable: false),
                    NbExtractedUsers = table.Column<int>(type: "int", nullable: false),
                    NbExtractedGroups = table.Column<int>(type: "int", nullable: false),
                    NbFilteredRepresentations = table.Column<int>(type: "int", nullable: false),
                    NbImportedPages = table.Column<int>(type: "int", nullable: false),
                    NbImportedGroups = table.Column<int>(type: "int", nullable: false),
                    NbImportedUsers = table.Column<int>(type: "int", nullable: false),
                    TotalPageToExtract = table.Column<int>(type: "int", nullable: false),
                    TotalPageToImport = table.Column<int>(type: "int", nullable: false),
                    IdentityProvisioningId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
