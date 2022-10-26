using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.OpenID.SqlServer.Startup.Migrations
{
    public partial class AddConsentIsDisabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConsentDisabled",
                table: "OpenIdClients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConsentDisabled",
                table: "OpenIdClients");
        }
    }
}
