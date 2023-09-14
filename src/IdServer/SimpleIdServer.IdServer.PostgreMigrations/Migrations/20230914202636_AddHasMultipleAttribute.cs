using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddHasMultipleAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasMultipleAttribute",
                table: "IdentityProvisioningMappingRule",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasMultipleAttribute",
                table: "IdentityProvisioningMappingRule");
        }
    }
}
