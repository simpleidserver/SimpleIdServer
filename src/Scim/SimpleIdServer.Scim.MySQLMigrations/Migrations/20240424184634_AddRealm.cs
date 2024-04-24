using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.MySQLMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddRealm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "SCIMSchemaLst",
                schema: "scim",
                newName: "SCIMSchemaLst",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SCIMSchemaExtension",
                schema: "scim",
                newName: "SCIMSchemaExtension",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SCIMSchemaAttribute",
                schema: "scim",
                newName: "SCIMSchemaAttribute",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SCIMRepresentationSCIMSchema",
                schema: "scim",
                newName: "SCIMRepresentationSCIMSchema",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SCIMRepresentationLst",
                schema: "scim",
                newName: "SCIMRepresentationLst",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SCIMRepresentationAttributeLst",
                schema: "scim",
                newName: "SCIMRepresentationAttributeLst",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SCIMAttributeMappingLst",
                schema: "scim",
                newName: "SCIMAttributeMappingLst",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ProvisioningConfigurations",
                schema: "scim",
                newName: "ProvisioningConfigurations",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ProvisioningConfigurationRecord",
                schema: "scim",
                newName: "ProvisioningConfigurationRecord",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "ProvisioningConfigurationHistory",
                schema: "scim",
                newName: "ProvisioningConfigurationHistory",
                newSchema: "dbo");

            migrationBuilder.AddColumn<string>(
                name: "RealmName",
                schema: "dbo",
                table: "SCIMRepresentationLst",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Realms",
                schema: "dbo",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Owner = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Realms",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "RealmName",
                schema: "dbo",
                table: "SCIMRepresentationLst");

            migrationBuilder.EnsureSchema(
                name: "scim");

            migrationBuilder.RenameTable(
                name: "SCIMSchemaLst",
                schema: "dbo",
                newName: "SCIMSchemaLst",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "SCIMSchemaExtension",
                schema: "dbo",
                newName: "SCIMSchemaExtension",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "SCIMSchemaAttribute",
                schema: "dbo",
                newName: "SCIMSchemaAttribute",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "SCIMRepresentationSCIMSchema",
                schema: "dbo",
                newName: "SCIMRepresentationSCIMSchema",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "SCIMRepresentationLst",
                schema: "dbo",
                newName: "SCIMRepresentationLst",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "SCIMRepresentationAttributeLst",
                schema: "dbo",
                newName: "SCIMRepresentationAttributeLst",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "SCIMAttributeMappingLst",
                schema: "dbo",
                newName: "SCIMAttributeMappingLst",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "ProvisioningConfigurations",
                schema: "dbo",
                newName: "ProvisioningConfigurations",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "ProvisioningConfigurationRecord",
                schema: "dbo",
                newName: "ProvisioningConfigurationRecord",
                newSchema: "scim");

            migrationBuilder.RenameTable(
                name: "ProvisioningConfigurationHistory",
                schema: "dbo",
                newName: "ProvisioningConfigurationHistory",
                newSchema: "scim");
        }
    }
}
