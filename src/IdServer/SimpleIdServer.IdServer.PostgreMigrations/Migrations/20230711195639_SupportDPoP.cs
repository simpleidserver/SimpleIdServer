using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class SupportDPoP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Jkt",
                table: "Tokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DPOPBoundAccessTokens",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "DPOPNonceLifetimeInSeconds",
                table: "Clients",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDPOPNonceRequired",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Jkt",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "DPOPBoundAccessTokens",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DPOPNonceLifetimeInSeconds",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsDPOPNonceRequired",
                table: "Clients");
        }
    }
}
