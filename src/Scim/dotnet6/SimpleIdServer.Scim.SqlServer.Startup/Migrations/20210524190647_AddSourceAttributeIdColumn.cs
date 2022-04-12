using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class AddSourceAttributeIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceAttributeId",
                table: "SCIMAttributeMappingLst",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceAttributeId",
                table: "SCIMAttributeMappingLst");
        }
    }
}
