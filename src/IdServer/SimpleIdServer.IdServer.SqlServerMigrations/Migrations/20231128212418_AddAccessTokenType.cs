using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
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
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessTokenType",
                table: "Clients",
                type: "int",
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
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
