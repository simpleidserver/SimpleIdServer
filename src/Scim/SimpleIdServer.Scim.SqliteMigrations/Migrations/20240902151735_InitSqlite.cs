using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.SqliteMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "scim");

            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurations",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourceType = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                schema: "scim",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Owner = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SCIMAttributeMappingLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SourceAttributeId = table.Column<string>(type: "TEXT", nullable: true),
                    SourceResourceType = table.Column<string>(type: "TEXT", nullable: true),
                    SourceAttributeSelector = table.Column<string>(type: "TEXT", nullable: true),
                    TargetResourceType = table.Column<string>(type: "TEXT", nullable: true),
                    TargetAttributeId = table.Column<string>(type: "TEXT", nullable: true),
                    Mode = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMAttributeMappingLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: true),
                    ResourceType = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    RealmName = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsRootSchema = table.Column<bool>(type: "INTEGER", nullable: false),
                    ResourceType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurationHistory",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RepresentationId = table.Column<string>(type: "TEXT", nullable: true),
                    RepresentationVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    WorkflowInstanceId = table.Column<string>(type: "TEXT", nullable: true),
                    WorkflowId = table.Column<string>(type: "TEXT", nullable: true),
                    ExecutionDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Exception = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ProvisioningConfigurationId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId",
                        column: x => x.ProvisioningConfigurationId,
                        principalSchema: "scim",
                        principalTable: "ProvisioningConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurationRecord",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArray = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValuesString = table.Column<string>(type: "TEXT", nullable: true),
                    ProvisioningConfigurationId = table.Column<string>(type: "TEXT", nullable: true),
                    ProvisioningConfigurationRecordId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurationRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurationRecord_ProvisioningConfigurationRecordId",
                        column: x => x.ProvisioningConfigurationRecordId,
                        principalSchema: "scim",
                        principalTable: "ProvisioningConfigurationRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId",
                        column: x => x.ProvisioningConfigurationId,
                        principalSchema: "scim",
                        principalTable: "ProvisioningConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationSCIMSchema",
                schema: "scim",
                columns: table => new
                {
                    RepresentationsId = table.Column<string>(type: "TEXT", nullable: false),
                    SchemasId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationSCIMSchema", x => new { x.RepresentationsId, x.SchemasId });
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationSCIMSchema_SCIMRepresentationLst_RepresentationsId",
                        column: x => x.RepresentationsId,
                        principalSchema: "scim",
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationSCIMSchema_SCIMSchemaLst_SchemasId",
                        column: x => x.SchemasId,
                        principalSchema: "scim",
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaAttribute",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FullPath = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<string>(type: "TEXT", nullable: true),
                    SchemaId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    MultiValued = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Required = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanonicalValues = table.Column<string>(type: "TEXT", nullable: true),
                    CaseExact = table.Column<bool>(type: "INTEGER", nullable: false),
                    Mutability = table.Column<int>(type: "INTEGER", nullable: false),
                    Returned = table.Column<int>(type: "INTEGER", nullable: false),
                    Uniqueness = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferenceTypes = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultValueString = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultValueInt = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaAttribute_SCIMSchemaLst_SchemaId",
                        column: x => x.SchemaId,
                        principalSchema: "scim",
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaExtension",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Schema = table.Column<string>(type: "TEXT", nullable: true),
                    Required = table.Column<bool>(type: "INTEGER", nullable: false),
                    SCIMSchemaId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaExtension", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaExtension_SCIMSchemaLst_SCIMSchemaId",
                        column: x => x.SCIMSchemaId,
                        principalSchema: "scim",
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationAttributeLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AttributeId = table.Column<string>(type: "TEXT", nullable: true),
                    ResourceType = table.Column<string>(type: "TEXT", nullable: true),
                    ParentAttributeId = table.Column<string>(type: "TEXT", nullable: true),
                    SchemaAttributeId = table.Column<string>(type: "TEXT", nullable: true),
                    RepresentationId = table.Column<string>(type: "TEXT", nullable: true),
                    FullPath = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    ValueString = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ValueBoolean = table.Column<bool>(type: "INTEGER", nullable: true),
                    ValueInteger = table.Column<int>(type: "INTEGER", nullable: true),
                    ValueDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValueReference = table.Column<string>(type: "TEXT", nullable: true),
                    ValueDecimal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    ValueBinary = table.Column<string>(type: "TEXT", nullable: true),
                    Namespace = table.Column<string>(type: "TEXT", nullable: true),
                    ComputedValueIndex = table.Column<string>(type: "TEXT", nullable: true),
                    IsComputed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationAttributeLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentAttributeId",
                        column: x => x.ParentAttributeId,
                        principalSchema: "scim",
                        principalTable: "SCIMRepresentationAttributeLst",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_RepresentationId",
                        column: x => x.RepresentationId,
                        principalSchema: "scim",
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttribute_SchemaAttributeId",
                        column: x => x.SchemaAttributeId,
                        principalSchema: "scim",
                        principalTable: "SCIMSchemaAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationHistory_ProvisioningConfigurationId",
                schema: "scim",
                table: "ProvisioningConfigurationHistory",
                column: "ProvisioningConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationRecord_ProvisioningConfigurationId",
                schema: "scim",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationRecord_ProvisioningConfigurationRecordId",
                schema: "scim",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_FullPath",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                column: "FullPath");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_ParentAttributeId",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                column: "ParentAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_RepresentationId",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                column: "RepresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_SchemaAttributeId",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                column: "SchemaAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_ValueString",
                schema: "scim",
                table: "SCIMRepresentationAttributeLst",
                column: "ValueString");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationSCIMSchema_SchemasId",
                schema: "scim",
                table: "SCIMRepresentationSCIMSchema",
                column: "SchemasId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaAttribute_SchemaId",
                schema: "scim",
                table: "SCIMSchemaAttribute",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaExtension_SCIMSchemaId",
                schema: "scim",
                table: "SCIMSchemaExtension",
                column: "SCIMSchemaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProvisioningConfigurationHistory",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "ProvisioningConfigurationRecord",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "Realms",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "SCIMAttributeMappingLst",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationAttributeLst",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationSCIMSchema",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "SCIMSchemaExtension",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "ProvisioningConfigurations",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "SCIMSchemaAttribute",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationLst",
                schema: "scim");

            migrationBuilder.DropTable(
                name: "SCIMSchemaLst",
                schema: "scim");
        }
    }
}
