using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
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
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDateTime",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FederationEntities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Realm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSubordinate = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
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
