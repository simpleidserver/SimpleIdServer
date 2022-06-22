using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class IgnoreChildren : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentAttributeId",
                table: "SCIMRepresentationAttributeLst");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentAttributeId",
                table: "SCIMRepresentationAttributeLst",
                column: "ParentAttributeId",
                principalTable: "SCIMRepresentationAttributeLst",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
