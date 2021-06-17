using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class UpdateMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceValueAttributeId",
                table: "SCIMAttributeMappingLst",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceValueAttributeId",
                table: "SCIMAttributeMappingLst");
        }
    }
}
