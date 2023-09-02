using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddComputedIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComputedValueIndex",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComputedValueIndex",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst");
        }
    }
}
