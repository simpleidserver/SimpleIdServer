using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdProvisioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityProvisioningDefinitionProperty");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningProperty");

            migrationBuilder.AddColumn<string>(
                name: "OptionsFullQualifiedName",
                table: "IdentityProvisioningDefinitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionsName",
                table: "IdentityProvisioningDefinitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionsFullQualifiedName",
                table: "IdentityProvisioningDefinitions");

            migrationBuilder.DropColumn(
                name: "OptionsName",
                table: "IdentityProvisioningDefinitions");

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningDefinitionProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityProvisioningDefinitionName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningDefinitionProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningDefinitionProperty_IdentityProvisioningDefinitions_IdentityProvisioningDefinitionName",
                        column: x => x.IdentityProvisioningDefinitionName,
                        principalTable: "IdentityProvisioningDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityProvisioningId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningProperty_IdentityProvisioningLst_IdentityProvisioningId",
                        column: x => x.IdentityProvisioningId,
                        principalTable: "IdentityProvisioningLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningDefinitionProperty_IdentityProvisioningDefinitionName",
                table: "IdentityProvisioningDefinitionProperty",
                column: "IdentityProvisioningDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningProperty_IdentityProvisioningId",
                table: "IdentityProvisioningProperty",
                column: "IdentityProvisioningId");
        }
    }
}
