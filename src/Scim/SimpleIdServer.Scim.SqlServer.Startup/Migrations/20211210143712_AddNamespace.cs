using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class AddNamespace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ParentAttributeId",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Namespace",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_ParentAttributeId",
                table: "SCIMRepresentationAttributeLst",
                column: "ParentAttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentAttributeId",
                table: "SCIMRepresentationAttributeLst",
                column: "ParentAttributeId",
                principalTable: "SCIMRepresentationAttributeLst",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentAttributeId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropIndex(
                name: "IX_SCIMRepresentationAttributeLst_ParentAttributeId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "Namespace",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.AlterColumn<string>(
                name: "ParentAttributeId",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
