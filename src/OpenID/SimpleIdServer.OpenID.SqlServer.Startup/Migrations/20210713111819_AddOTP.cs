using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.OpenID.SqlServer.Startup.Migrations
{
    public partial class AddOTP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OTPCounter",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OTPKey",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTPCounter",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OTPKey",
                table: "Users");
        }
    }
}
