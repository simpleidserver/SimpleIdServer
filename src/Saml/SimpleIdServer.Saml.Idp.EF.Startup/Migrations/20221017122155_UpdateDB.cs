using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Saml.Idp.EF.Startup.Migrations
{
    public partial class UpdateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssertionExpirationBeforeInSeconds",
                table: "RelyingParties",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssertionExpirationBeforeInSeconds",
                table: "RelyingParties");
        }
    }
}
