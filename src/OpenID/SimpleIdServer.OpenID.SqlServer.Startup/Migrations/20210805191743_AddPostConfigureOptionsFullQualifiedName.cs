using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.OpenID.SqlServer.Startup.Migrations
{
    public partial class AddPostConfigureOptionsFullQualifiedName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostConfigureOptionsFullQualifiedName",
                table: "AuthenticationSchemeProviders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostConfigureOptionsFullQualifiedName",
                table: "AuthenticationSchemeProviders");
        }
    }
}
