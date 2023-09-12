using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class SupportDynamicConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderDefinitionProperty");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderProperty");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningDefinitionProperty");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningProperty");

            migrationBuilder.AddColumn<string>(
                name: "OptionsFullQualifiedName",
                table: "IdentityProvisioningDefinitions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionsName",
                table: "IdentityProvisioningDefinitions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionsName",
                table: "AuthenticationSchemeProviderDefinitions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfigurationKeyPairValueRecords",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationKeyPairValueRecords", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Definitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FullQualifiedName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DisplayCondition = table.Column<string>(type: "text", nullable: true),
                    ConfigurationDefinitionId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecord_Definitions_ConfigurationDefi~",
                        column: x => x.ConfigurationDefinitionId,
                        principalTable: "Definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordTranslation",
                columns: table => new
                {
                    ConfigurationDefinitionRecordId = table.Column<string>(type: "text", nullable: false),
                    TranslationsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordTranslation", x => new { x.ConfigurationDefinitionRecordId, x.TranslationsId });
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordTranslation_ConfigurationDefin~",
                        column: x => x.ConfigurationDefinitionRecordId,
                        principalTable: "ConfigurationDefinitionRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordTranslation_Translations_Trans~",
                        column: x => x.TranslationsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordValue",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ConfigurationDefinitionRecordId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValue_ConfigurationDefinitionR~",
                        column: x => x.ConfigurationDefinitionRecordId,
                        principalTable: "ConfigurationDefinitionRecord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordValueTranslation",
                columns: table => new
                {
                    ConfigurationDefinitionRecordValueId = table.Column<string>(type: "text", nullable: false),
                    TranslationsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordValueTranslation", x => new { x.ConfigurationDefinitionRecordValueId, x.TranslationsId });
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValueTranslation_Configuration~",
                        column: x => x.ConfigurationDefinitionRecordValueId,
                        principalTable: "ConfigurationDefinitionRecordValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValueTranslation_Translations_~",
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
                name: "IX_ConfigurationDefinitionRecordValue_ConfigurationDefinitionR~",
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

            migrationBuilder.DropColumn(
                name: "OptionsFullQualifiedName",
                table: "IdentityProvisioningDefinitions");

            migrationBuilder.DropColumn(
                name: "OptionsName",
                table: "IdentityProvisioningDefinitions");

            migrationBuilder.DropColumn(
                name: "OptionsName",
                table: "AuthenticationSchemeProviderDefinitions");

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderDefinitionProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchemeProviderDefName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderDefinitionProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderDefinitionProperty_Authenticati~",
                        column: x => x.SchemeProviderDefName,
                        principalTable: "AuthenticationSchemeProviderDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchemeProviderId = table.Column<string>(type: "text", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderProperty_AuthenticationSchemePr~",
                        column: x => x.SchemeProviderId,
                        principalTable: "AuthenticationSchemeProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningDefinitionProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdentityProvisioningDefinitionName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningDefinitionProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningDefinitionProperty_IdentityProvisioning~",
                        column: x => x.IdentityProvisioningDefinitionName,
                        principalTable: "IdentityProvisioningDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdentityProvisioningId = table.Column<string>(type: "text", nullable: false),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningProperty_IdentityProvisioningLst_Identi~",
                        column: x => x.IdentityProvisioningId,
                        principalTable: "IdentityProvisioningLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderDefinitionProperty_SchemeProvid~",
                table: "AuthenticationSchemeProviderDefinitionProperty",
                column: "SchemeProviderDefName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderProperty_SchemeProviderId",
                table: "AuthenticationSchemeProviderProperty",
                column: "SchemeProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningDefinitionProperty_IdentityProvisioning~",
                table: "IdentityProvisioningDefinitionProperty",
                column: "IdentityProvisioningDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningProperty_IdentityProvisioningId",
                table: "IdentityProvisioningProperty",
                column: "IdentityProvisioningId");
        }
    }
}
