using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.MySQLMigrations.Migrations
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
                type: "tinyint(1)",
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
