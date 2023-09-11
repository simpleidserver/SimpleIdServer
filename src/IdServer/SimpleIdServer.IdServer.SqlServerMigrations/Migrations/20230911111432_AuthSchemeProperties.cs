using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AuthSchemeProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderDefinitionProperty");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderProperty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderDefinitionProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemeProviderDefName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderDefinitionProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderDefinitionProperty_AuthenticationSchemeProviderDefinitions_SchemeProviderDefName",
                        column: x => x.SchemeProviderDefName,
                        principalTable: "AuthenticationSchemeProviderDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemeProviderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderProperty_AuthenticationSchemeProviders_SchemeProviderId",
                        column: x => x.SchemeProviderId,
                        principalTable: "AuthenticationSchemeProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderDefinitionProperty_SchemeProviderDefName",
                table: "AuthenticationSchemeProviderDefinitionProperty",
                column: "SchemeProviderDefName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderProperty_SchemeProviderId",
                table: "AuthenticationSchemeProviderProperty",
                column: "SchemeProviderId");
        }
    }
}
