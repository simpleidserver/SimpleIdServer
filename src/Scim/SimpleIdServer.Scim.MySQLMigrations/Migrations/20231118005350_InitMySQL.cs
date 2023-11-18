using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.MySQLMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitMySQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "scim");

            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurations",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ResourceType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SCIMAttributeMappingLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceAttributeId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceResourceType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceAttributeSelector = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetResourceType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetAttributeId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMAttributeMappingLst", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExternalId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResourceType = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Version = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationLst", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SCIMSchemaLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRootSchema = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ResourceType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaLst", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurationHistory",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RepresentationId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RepresentationVersion = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WorkflowInstanceId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WorkflowId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExecutionDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Exception = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProvisioningConfigurationId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_~",
                        column: x => x.ProvisioningConfigurationId,
                        principalSchema: "scim",
                        principalTable: "ProvisioningConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurationRecord",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsArray = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ValuesString = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProvisioningConfigurationId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProvisioningConfigurationRecordId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurationRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurationRec~",
                        column: x => x.ProvisioningConfigurationRecordId,
                        principalSchema: "scim",
                        principalTable: "ProvisioningConfigurationRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_P~",
                        column: x => x.ProvisioningConfigurationId,
                        principalSchema: "scim",
                        principalTable: "ProvisioningConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationSCIMSchema",
                schema: "scim",
                columns: table => new
                {
                    RepresentationsId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SchemasId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationSCIMSchema", x => new { x.RepresentationsId, x.SchemasId });
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationSCIMSchema_SCIMRepresentationLst_Represent~",
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SCIMSchemaAttribute",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SchemaId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MultiValued = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Required = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanonicalValues = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CaseExact = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Mutability = table.Column<int>(type: "int", nullable: false),
                    Returned = table.Column<int>(type: "int", nullable: false),
                    Uniqueness = table.Column<int>(type: "int", nullable: false),
                    ReferenceTypes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultValueString = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultValueInt = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SCIMSchemaExtension",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Schema = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Required = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SCIMSchemaId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationAttributeLst",
                schema: "scim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AttributeId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResourceType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentAttributeId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SchemaAttributeId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RepresentationId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullPath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValueString = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValueBoolean = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ValueInteger = table.Column<int>(type: "int", nullable: true),
                    ValueDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ValueReference = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValueDecimal = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ValueBinary = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Namespace = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ComputedValueIndex = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsComputed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationAttributeLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLs~",
                        column: x => x.ParentAttributeId,
                        principalSchema: "scim",
                        principalTable: "SCIMRepresentationAttributeLst",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_Represe~",
                        column: x => x.RepresentationId,
                        principalSchema: "scim",
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttribute_SchemaAtt~",
                        column: x => x.SchemaAttributeId,
                        principalSchema: "scim",
                        principalTable: "SCIMSchemaAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "IX_ProvisioningConfigurationRecord_ProvisioningConfigurationRec~",
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
