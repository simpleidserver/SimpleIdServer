using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqlServerMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationKeyPairValueRecords",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationKeyPairValueRecords", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Definitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfigurationDefinitionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecord_Definitions_ConfigurationDefinitionId",
                        column: x => x.ConfigurationDefinitionId,
                        principalTable: "Definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordTranslation",
                columns: table => new
                {
                    ConfigurationDefinitionRecordId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TranslationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordTranslation", x => new { x.ConfigurationDefinitionRecordId, x.TranslationsId });
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordTranslation_ConfigurationDefinitionRecord_ConfigurationDefinitionRecordId",
                        column: x => x.ConfigurationDefinitionRecordId,
                        principalTable: "ConfigurationDefinitionRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordTranslation_Translations_TranslationsId",
                        column: x => x.TranslationsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordValue",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfigurationDefinitionRecordId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecord_ConfigurationDefinitionRecordId",
                        column: x => x.ConfigurationDefinitionRecordId,
                        principalTable: "ConfigurationDefinitionRecord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordValueTranslation",
                columns: table => new
                {
                    ConfigurationDefinitionRecordValueId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TranslationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordValueTranslation", x => new { x.ConfigurationDefinitionRecordValueId, x.TranslationsId });
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                        column: x => x.ConfigurationDefinitionRecordValueId,
                        principalTable: "ConfigurationDefinitionRecordValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValueTranslation_Translations_TranslationsId",
                        column: x => x.TranslationsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDefinitionRecord_ConfigurationDefinitionId",
                table: "ConfigurationDefinitionRecord",
                column: "ConfigurationDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDefinitionRecordTranslation_TranslationsId",
                table: "ConfigurationDefinitionRecordTranslation",
                column: "TranslationsId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordId",
                table: "ConfigurationDefinitionRecordValue",
                column: "ConfigurationDefinitionRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDefinitionRecordValueTranslation_TranslationsId",
                table: "ConfigurationDefinitionRecordValueTranslation",
                column: "TranslationsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecordTranslation");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecordValueTranslation");

            migrationBuilder.DropTable(
                name: "ConfigurationKeyPairValueRecords");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecordValue");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecord");

            migrationBuilder.DropTable(
                name: "Definitions");
        }
    }
}
