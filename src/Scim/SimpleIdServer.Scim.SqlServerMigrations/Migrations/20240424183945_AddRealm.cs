using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddRealm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RealmName",
                schema: "scim",
                table: "SCIMRepresentationLst",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Realms",
                schema: "scim",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Realms",
                schema: "scim");

            migrationBuilder.DropColumn(
                name: "RealmName",
                schema: "scim",
                table: "SCIMRepresentationLst");
        }
    }
}
