using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIntVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Version",
                schema: "scim",
                table: "SCIMRepresentationLst",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "ValueDecimal",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "scim",
                table: "SCIMRepresentationLst",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ValueDecimal",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
