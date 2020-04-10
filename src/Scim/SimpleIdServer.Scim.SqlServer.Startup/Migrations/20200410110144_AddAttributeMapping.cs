using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class AddAttributeMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetAttributeSelector",
                table: "SCIMAttributeMappingLst",
                newName: "TargetAttributeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetAttributeId",
                table: "SCIMAttributeMappingLst",
                newName: "TargetAttributeSelector");
        }
    }
}
