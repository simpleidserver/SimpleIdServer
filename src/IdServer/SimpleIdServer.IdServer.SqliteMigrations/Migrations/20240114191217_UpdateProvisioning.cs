using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqliteMigrations.Migrations
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
                newName: "ProcessId");

            migrationBuilder.RenameColumn(
                name: "NbRepresentations",
                table: "IdentityProvisioningHistory",
                newName: "TotalPages");

            migrationBuilder.AddColumn<int>(
                name: "Usage",
                table: "IdentityProvisioningMappingRule",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentPage",
                table: "IdentityProvisioningHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionDateTime",
                table: "IdentityProvisioningHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NbFilteredRepresentations",
                table: "IdentityProvisioningHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NbGroups",
                table: "IdentityProvisioningHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NbUsers",
                table: "IdentityProvisioningHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExtractedRepresentationsStaging",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    RepresentationId = table.Column<string>(type: "TEXT", nullable: false),
                    RepresentationVersion = table.Column<string>(type: "TEXT", nullable: false),
                    Values = table.Column<string>(type: "TEXT", nullable: true),
                    IdProvisioningProcessId = table.Column<string>(type: "TEXT", nullable: false),
                    GroupIds = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
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
                name: "ExecutionDateTime",
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

            migrationBuilder.RenameColumn(
                name: "TotalPages",
                table: "IdentityProvisioningHistory",
                newName: "NbRepresentations");

            migrationBuilder.RenameColumn(
                name: "ProcessId",
                table: "IdentityProvisioningHistory",
                newName: "StartDateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "IdentityProvisioningHistory",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "IdentityProvisioningHistory",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FolderName",
                table: "IdentityProvisioningHistory",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImportSummaries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    RealmName = table.Column<string>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    NbRepresentations = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
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
