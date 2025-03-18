using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqliteMigrations.Migrations
{
    /// <inheritdoc />
    public partial class FixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                table: "ConfigurationDefinitionRecordValueTranslation");

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationDefinitionRecordValueId",
                table: "Translations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationDefinitionRecordValueId1",
                table: "Translations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullQualifiedName",
                table: "Definitions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ConfigurationKeyPairValueRecords",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ConfigurationDefinitionRecordValue",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ConfigurationDefinitionRecord",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordValues",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordValues", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_ConfigurationDefinitionRecordValueId",
                table: "Translations",
                column: "ConfigurationDefinitionRecordValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_ConfigurationDefinitionRecordValueId1",
                table: "Translations",
                column: "ConfigurationDefinitionRecordValueId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValues_ConfigurationDefinitionRecordValueId",
                table: "ConfigurationDefinitionRecordValueTranslation",
                column: "ConfigurationDefinitionRecordValueId",
                principalTable: "ConfigurationDefinitionRecordValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                table: "Translations",
                column: "ConfigurationDefinitionRecordValueId",
                principalTable: "ConfigurationDefinitionRecordValue",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId1",
                table: "Translations",
                column: "ConfigurationDefinitionRecordValueId1",
                principalTable: "ConfigurationDefinitionRecordValue",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValues_ConfigurationDefinitionRecordValueId",
                table: "ConfigurationDefinitionRecordValueTranslation");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                table: "Translations");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId1",
                table: "Translations");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecordValues");

            migrationBuilder.DropIndex(
                name: "IX_Translations_ConfigurationDefinitionRecordValueId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_ConfigurationDefinitionRecordValueId1",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "ConfigurationDefinitionRecordValueId",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "ConfigurationDefinitionRecordValueId1",
                table: "Translations");

            migrationBuilder.AlterColumn<string>(
                name: "FullQualifiedName",
                table: "Definitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ConfigurationKeyPairValueRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ConfigurationDefinitionRecordValue",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ConfigurationDefinitionRecord",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                table: "ConfigurationDefinitionRecordValueTranslation",
                column: "ConfigurationDefinitionRecordValueId",
                principalTable: "ConfigurationDefinitionRecordValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
