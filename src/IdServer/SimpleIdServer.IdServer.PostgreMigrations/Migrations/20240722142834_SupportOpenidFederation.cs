using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class SupportOpenidFederation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientRegistrationTypesSupported",
                table: "Clients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDateTime",
                table: "Clients",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FederationEntities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Sub = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: true),
                    IsSubordinate = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FederationEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FederationEntities");

            migrationBuilder.DropColumn(
                name: "ClientRegistrationTypesSupported",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ExpirationDateTime",
                table: "Clients");
        }
    }
}
