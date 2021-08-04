using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.OpenID.SqlServer.Startup.Migrations
{
    public partial class UpdateAuthSchemeprovider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonConverter",
                table: "AuthenticationSchemeProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionsFullQualifiedName",
                table: "AuthenticationSchemeProviders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonConverter",
                table: "AuthenticationSchemeProviders");

            migrationBuilder.DropColumn(
                name: "OptionsFullQualifiedName",
                table: "AuthenticationSchemeProviders");
        }
    }
}
