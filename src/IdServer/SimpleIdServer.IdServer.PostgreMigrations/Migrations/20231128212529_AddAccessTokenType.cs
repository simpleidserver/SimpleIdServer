using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessTokenType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRegistrationAccessToken",
                table: "Tokens");

            migrationBuilder.AddColumn<int>(
                name: "AccessTokenType",
                table: "Tokens",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessTokenType",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessTokenType",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "AccessTokenType",
                table: "Clients");

            migrationBuilder.AddColumn<bool>(
                name: "IsRegistrationAccessToken",
                table: "Tokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
