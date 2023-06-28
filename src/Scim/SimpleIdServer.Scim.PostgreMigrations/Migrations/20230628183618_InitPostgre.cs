using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimpleIdServer.Scim.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitPostgre : Migration
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<string>(type: "text", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SCIMAttributeMappingLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    SourceAttributeId = table.Column<string>(type: "text", nullable: true),
                    SourceResourceType = table.Column<string>(type: "text", nullable: true),
                    SourceAttributeSelector = table.Column<string>(type: "text", nullable: true),
                    TargetResourceType = table.Column<string>(type: "text", nullable: true),
                    TargetAttributeId = table.Column<string>(type: "text", nullable: true),
                    Mode = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    ResourceType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsRootSchema = table.Column<bool>(type: "boolean", nullable: false),
                    ResourceType = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RepresentationId = table.Column<string>(type: "text", nullable: true),
                    RepresentationVersion = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    WorkflowInstanceId = table.Column<string>(type: "text", nullable: true),
                    WorkflowId = table.Column<string>(type: "text", nullable: true),
                    ExecutionDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProvisioningConfigurationId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations~",
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsArray = table.Column<bool>(type: "boolean", nullable: false),
                    ValuesString = table.Column<string>(type: "text", nullable: true),
                    ProvisioningConfigurationId = table.Column<string>(type: "text", nullable: true),
                    ProvisioningConfigurationRecordId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurationRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurationRe~",
                        column: x => x.ProvisioningConfigurationRecordId,
                        principalSchema: "scim",
                        principalTable: "ProvisioningConfigurationRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_~",
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
                    RepresentationsId = table.Column<string>(type: "text", nullable: false),
                    SchemasId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationSCIMSchema", x => new { x.RepresentationsId, x.SchemasId });
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationSCIMSchema_SCIMRepresentationLst_Represen~",
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullPath = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<string>(type: "text", nullable: true),
                    SchemaId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MultiValued = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    CanonicalValues = table.Column<string>(type: "text", nullable: true),
                    CaseExact = table.Column<bool>(type: "boolean", nullable: false),
                    Mutability = table.Column<int>(type: "integer", nullable: false),
                    Returned = table.Column<int>(type: "integer", nullable: false),
                    Uniqueness = table.Column<int>(type: "integer", nullable: false),
                    ReferenceTypes = table.Column<string>(type: "text", nullable: true),
                    DefaultValueString = table.Column<string>(type: "text", nullable: true),
                    DefaultValueInt = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Schema = table.Column<string>(type: "text", nullable: true),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    SCIMSchemaId = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    AttributeId = table.Column<string>(type: "text", nullable: true),
                    ResourceType = table.Column<string>(type: "text", nullable: true),
                    ParentAttributeId = table.Column<string>(type: "text", nullable: true),
                    SchemaAttributeId = table.Column<string>(type: "text", nullable: true),
                    RepresentationId = table.Column<string>(type: "text", nullable: true),
                    FullPath = table.Column<string>(type: "text", nullable: true),
                    ValueString = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ValueBoolean = table.Column<bool>(type: "boolean", nullable: true),
                    ValueInteger = table.Column<int>(type: "integer", nullable: true),
                    ValueDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValueReference = table.Column<string>(type: "text", nullable: true),
                    ValueDecimal = table.Column<decimal>(type: "numeric", nullable: true),
                    ValueBinary = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationAttributeLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeL~",
                        column: x => x.ParentAttributeId,
                        principalSchema: "scim",
                        principalTable: "SCIMRepresentationAttributeLst",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_Repres~",
                        column: x => x.RepresentationId,
                        principalSchema: "scim",
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttribute_SchemaAt~",
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
                name: "IX_ProvisioningConfigurationRecord_ProvisioningConfigurationRe~",
                schema: "scim",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationRecordId");

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
