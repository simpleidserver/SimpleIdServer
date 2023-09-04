using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComputedValueIndex",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsComputed",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComputedValueIndex",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "IsComputed",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst");
        }
    }
}
