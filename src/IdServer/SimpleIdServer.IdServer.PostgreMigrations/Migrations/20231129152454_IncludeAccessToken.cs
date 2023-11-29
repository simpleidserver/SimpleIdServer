using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class IncludeAccessToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludeInAccessToken",
                table: "ScopeClaimMapper",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeInAccessToken",
                table: "ScopeClaimMapper");
        }
    }
}
