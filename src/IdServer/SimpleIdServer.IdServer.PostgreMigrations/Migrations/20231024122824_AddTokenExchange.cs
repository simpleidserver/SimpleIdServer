using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenExchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Resources",
                table: "AuthorizedScope");

            migrationBuilder.AddColumn<bool>(
                name: "IsTokenExchangeEnabled",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TokenExchangeType",
                table: "Clients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Audience",
                table: "ApiResources",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuthorizedResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Resource = table.Column<string>(type: "text", nullable: false),
                    Audience = table.Column<string>(type: "text", nullable: true),
                    AuthorizedScopeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizedResource_AuthorizedScope_AuthorizedScopeId",
                        column: x => x.AuthorizedScopeId,
                        principalTable: "AuthorizedScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedResource_AuthorizedScopeId",
                table: "AuthorizedResource",
                column: "AuthorizedScopeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizedResource");

            migrationBuilder.DropColumn(
                name: "IsTokenExchangeEnabled",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "TokenExchangeType",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Audience",
                table: "ApiResources");

            migrationBuilder.AddColumn<string>(
                name: "Resources",
                table: "AuthorizedScope",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
