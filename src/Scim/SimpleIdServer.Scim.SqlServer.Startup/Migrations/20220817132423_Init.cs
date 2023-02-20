using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SCIMAttributeMappingLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceAttributeSelector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMAttributeMappingLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRootSchema = table.Column<bool>(type: "bit", nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurationHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RepresentationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepresentationVersion = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowInstanceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProvisioningConfigurationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId",
                        column: x => x.ProvisioningConfigurationId,
                        principalTable: "ProvisioningConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProvisioningConfigurationRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsArray = table.Column<bool>(type: "bit", nullable: false),
                    ValuesString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvisioningConfigurationId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProvisioningConfigurationRecordId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningConfigurationRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurationRecord_ProvisioningConfigurationRecordId",
                        column: x => x.ProvisioningConfigurationRecordId,
                        principalTable: "ProvisioningConfigurationRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId",
                        column: x => x.ProvisioningConfigurationId,
                        principalTable: "ProvisioningConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationSCIMSchema",
                columns: table => new
                {
                    RepresentationsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SchemasId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationSCIMSchema", x => new { x.RepresentationsId, x.SchemasId });
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationSCIMSchema_SCIMRepresentationLst_RepresentationsId",
                        column: x => x.RepresentationsId,
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationSCIMSchema_SCIMSchemaLst_SchemasId",
                        column: x => x.SchemasId,
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaAttribute",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchemaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MultiValued = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    CanonicalValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaseExact = table.Column<bool>(type: "bit", nullable: false),
                    Mutability = table.Column<int>(type: "int", nullable: false),
                    Returned = table.Column<int>(type: "int", nullable: false),
                    Uniqueness = table.Column<int>(type: "int", nullable: false),
                    ReferenceTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultValueString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultValueInt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaAttribute_SCIMSchemaLst_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaExtension",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Schema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    SCIMSchemaId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaExtension", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaExtension_SCIMSchemaLst_SCIMSchemaId",
                        column: x => x.SCIMSchemaId,
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationAttributeLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentAttributeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SchemaAttributeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RepresentationId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueString = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ValueBoolean = table.Column<bool>(type: "bit", nullable: true),
                    ValueInteger = table.Column<int>(type: "int", nullable: true),
                    ValueDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValueReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueDecimal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValueBinary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Namespace = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationAttributeLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentAttributeId",
                        column: x => x.ParentAttributeId,
                        principalTable: "SCIMRepresentationAttributeLst",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_RepresentationId",
                        column: x => x.RepresentationId,
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttribute_SchemaAttributeId",
                        column: x => x.SchemaAttributeId,
                        principalTable: "SCIMSchemaAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationHistory_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationHistory",
                column: "ProvisioningConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationRecord_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationRecord_ProvisioningConfigurationRecordId",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_ParentAttributeId",
                table: "SCIMRepresentationAttributeLst",
                column: "ParentAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_RepresentationId",
                table: "SCIMRepresentationAttributeLst",
                column: "RepresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_SchemaAttributeId",
                table: "SCIMRepresentationAttributeLst",
                column: "SchemaAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationSCIMSchema_SchemasId",
                table: "SCIMRepresentationSCIMSchema",
                column: "SchemasId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaAttribute_SchemaId",
                table: "SCIMSchemaAttribute",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaExtension_SCIMSchemaId",
                table: "SCIMSchemaExtension",
                column: "SCIMSchemaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProvisioningConfigurationHistory");

            migrationBuilder.DropTable(
                name: "ProvisioningConfigurationRecord");

            migrationBuilder.DropTable(
                name: "SCIMAttributeMappingLst");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationSCIMSchema");

            migrationBuilder.DropTable(
                name: "SCIMSchemaExtension");

            migrationBuilder.DropTable(
                name: "ProvisioningConfigurations");

            migrationBuilder.DropTable(
                name: "SCIMSchemaAttribute");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationLst");

            migrationBuilder.DropTable(
                name: "SCIMSchemaLst");
        }
    }
}
