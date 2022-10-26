using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.OAuth.SqlServer.Startup.Migrations
{
    public partial class AddConsentIsDisabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConsentDisabled",
                table: "OAuthClients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConsentDisabled",
                table: "OAuthClients");
        }
    }
}
