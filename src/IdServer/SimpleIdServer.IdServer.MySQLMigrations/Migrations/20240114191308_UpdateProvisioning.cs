using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.MySQLMigrations
{
    /// <inheritdoc />
    public partial class UpdateProvisioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportSummaries");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "IdentityProvisioningHistory");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "IdentityProvisioningHistory");

            migrationBuilder.DropColumn(
                name: "FolderName",
                table: "IdentityProvisioningHistory");

            migrationBuilder.RenameColumn(
                name: "StartDateTime",
                table: "IdentityProvisioningHistory",
                newName: "ExecutionDateTime");

            migrationBuilder.RenameColumn(
                name: "NbRepresentations",
                table: "IdentityProvisioningHistory",
                newName: "TotalPages");

            migrationBuilder.AddColumn<int>(
                name: "Usage",
                table: "IdentityProvisioningMappingRule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentPage",
                table: "IdentityProvisioningHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NbFilteredRepresentations",
                table: "IdentityProvisioningHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NbGroups",
                table: "IdentityProvisioningHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NbUsers",
                table: "IdentityProvisioningHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProcessId",
                table: "IdentityProvisioningHistory",
                type: "longtext",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ExtractedRepresentationsStaging",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    RepresentationId = table.Column<string>(type: "longtext", nullable: false),
                    RepresentationVersion = table.Column<string>(type: "longtext", nullable: false),
                    Values = table.Column<string>(type: "longtext", nullable: true),
                    IdProvisioningProcessId = table.Column<string>(type: "longtext", nullable: false),
                    GroupIds = table.Column<string>(type: "longtext", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractedRepresentationsStaging", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtractedRepresentationsStaging");

            migrationBuilder.DropColumn(
                name: "Usage",
                table: "IdentityProvisioningMappingRule");

            migrationBuilder.DropColumn(
                name: "CurrentPage",
                table: "IdentityProvisioningHistory");

            migrationBuilder.DropColumn(
                name: "NbFilteredRepresentations",
                table: "IdentityProvisioningHistory");

            migrationBuilder.DropColumn(
                name: "NbGroups",
                table: "IdentityProvisioningHistory");

            migrationBuilder.DropColumn(
                name: "NbUsers",
                table: "IdentityProvisioningHistory");

            migrationBuilder.DropColumn(
                name: "ProcessId",
                table: "IdentityProvisioningHistory");

            migrationBuilder.RenameColumn(
                name: "TotalPages",
                table: "IdentityProvisioningHistory",
                newName: "NbRepresentations");

            migrationBuilder.RenameColumn(
                name: "ExecutionDateTime",
                table: "IdentityProvisioningHistory",
                newName: "StartDateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "IdentityProvisioningHistory",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "IdentityProvisioningHistory",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FolderName",
                table: "IdentityProvisioningHistory",
                type: "longtext",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImportSummaries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    RealmName = table.Column<string>(type: "varchar(255)", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "longtext", nullable: true),
                    NbRepresentations = table.Column<int>(type: "int", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportSummaries_Realms_RealmName",
                        column: x => x.RealmName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImportSummaries_RealmName",
                table: "ImportSummaries",
                column: "RealmName");
        }
    }
}
