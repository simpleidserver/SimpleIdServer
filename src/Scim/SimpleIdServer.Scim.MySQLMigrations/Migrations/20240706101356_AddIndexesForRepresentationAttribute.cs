using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.MySQLMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesForRepresentationAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullPath",
                schema: "dbo",
                table: "SCIMRepresentationAttributeLst",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_FullPath",
                schema: "dbo",
                table: "SCIMRepresentationAttributeLst",
                column: "FullPath");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_ValueString",
                schema: "dbo",
                table: "SCIMRepresentationAttributeLst",
                column: "ValueString");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SCIMRepresentationAttributeLst_FullPath",
                schema: "dbo",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropIndex(
                name: "IX_SCIMRepresentationAttributeLst_ValueString",
                schema: "dbo",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.AlterColumn<string>(
                name: "FullPath",
                schema: "dbo",
                table: "SCIMRepresentationAttributeLst",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
