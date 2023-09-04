using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIsComputed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsComputed",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsComputed",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst");
        }
    }
}
