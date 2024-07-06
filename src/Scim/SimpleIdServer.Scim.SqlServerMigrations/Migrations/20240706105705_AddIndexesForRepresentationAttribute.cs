using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesForRepresentationAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullPath",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_FullPath",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                column: "FullPath");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_ValueString",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                column: "ValueString");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SCIMRepresentationAttributeLst_FullPath",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropIndex(
                name: "IX_SCIMRepresentationAttributeLst_ValueString",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.AlterColumn<string>(
                name: "FullPath",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
