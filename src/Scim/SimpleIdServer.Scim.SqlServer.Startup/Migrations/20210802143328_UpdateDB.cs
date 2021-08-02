using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class UpdateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_RepresentationId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttributeModel_SchemaAttributeId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationAttributeValueLst");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationSchemaLst");

            migrationBuilder.DropTable(
                name: "SCIMSchemaAttributeModel");

            migrationBuilder.DropTable(
                name: "SCIMSchemaExtensionModel");

            migrationBuilder.DropIndex(
                name: "IX_SCIMRepresentationAttributeLst_ParentId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                table: "SCIMRepresentationLst",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttributeId",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullPath",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentAttributeId",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValueBinary",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ValueBoolean",
                table: "SCIMRepresentationAttributeLst",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValueDateTime",
                table: "SCIMRepresentationAttributeLst",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValueDecimal",
                table: "SCIMRepresentationAttributeLst",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValueInteger",
                table: "SCIMRepresentationAttributeLst",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValueReference",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValueString",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(max)",
                nullable: true);

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
                    ProvisioningConfigurationId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationHistory_ProvisioningConfigurations_ProvisioningConfigurationId1",
                        column: x => x.ProvisioningConfigurationId1,
                        principalTable: "ProvisioningConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProvisioningConfigurationRecord_ProvisioningConfigurations_ProvisioningConfigurationId",
                        column: x => x.ProvisioningConfigurationId,
                        principalTable: "ProvisioningConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationHistory_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationHistory",
                column: "ProvisioningConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationHistory_ProvisioningConfigurationId1",
                table: "ProvisioningConfigurationHistory",
                column: "ProvisioningConfigurationId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationRecord_ProvisioningConfigurationId",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningConfigurationRecord_ProvisioningConfigurationRecordId",
                table: "ProvisioningConfigurationRecord",
                column: "ProvisioningConfigurationRecordId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_RepresentationId",
                table: "SCIMRepresentationAttributeLst",
                column: "RepresentationId",
                principalTable: "SCIMRepresentationLst",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttribute_SchemaAttributeId",
                table: "SCIMRepresentationAttributeLst",
                column: "SchemaAttributeId",
                principalTable: "SCIMSchemaAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_RepresentationId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttribute_SchemaAttributeId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropTable(
                name: "ProvisioningConfigurationHistory");

            migrationBuilder.DropTable(
                name: "ProvisioningConfigurationRecord");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationSCIMSchema");

            migrationBuilder.DropTable(
                name: "SCIMSchemaAttribute");

            migrationBuilder.DropTable(
                name: "SCIMSchemaExtension");

            migrationBuilder.DropTable(
                name: "ProvisioningConfigurations");

            migrationBuilder.DropColumn(
                name: "AttributeId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "FullPath",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ParentAttributeId",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ValueBinary",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ValueBoolean",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ValueDateTime",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ValueDecimal",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ValueInteger",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ValueReference",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropColumn(
                name: "ValueString",
                table: "SCIMRepresentationAttributeLst");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "SCIMRepresentationLst",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "SCIMRepresentationAttributeLst",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationAttributeValueLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SCIMRepresentationAttributeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ValueBoolean = table.Column<bool>(type: "bit", nullable: true),
                    ValueByte = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ValueDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValueDecimal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValueInteger = table.Column<int>(type: "int", nullable: true),
                    ValueReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueString = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationAttributeValueLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeValueLst_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeId",
                        column: x => x.SCIMRepresentationAttributeId,
                        principalTable: "SCIMRepresentationAttributeLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationSchemaLst",
                columns: table => new
                {
                    SCIMSchemaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SCIMRepresentationId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationSchemaLst", x => new { x.SCIMSchemaId, x.SCIMRepresentationId });
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationSchemaLst_SCIMRepresentationLst_SCIMRepresentationId",
                        column: x => x.SCIMRepresentationId,
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationSchemaLst_SCIMSchemaLst_SCIMSchemaId",
                        column: x => x.SCIMSchemaId,
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaAttributeModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CanonicalValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaseExact = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValueInt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultValueString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MultiValued = table.Column<bool>(type: "bit", nullable: false),
                    Mutability = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReferenceTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    Returned = table.Column<int>(type: "int", nullable: false),
                    SchemaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Uniqueness = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaAttributeModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaAttributeModel_SCIMSchemaAttributeModel_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SCIMSchemaAttributeModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaAttributeModel_SCIMSchemaLst_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaExtensionModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    SCIMSchemaModelId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Schema = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaExtensionModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaExtensionModel_SCIMSchemaLst_SCIMSchemaModelId",
                        column: x => x.SCIMSchemaModelId,
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_ParentId",
                table: "SCIMRepresentationAttributeLst",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeValueLst_SCIMRepresentationAttributeId",
                table: "SCIMRepresentationAttributeValueLst",
                column: "SCIMRepresentationAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationSchemaLst_SCIMRepresentationId",
                table: "SCIMRepresentationSchemaLst",
                column: "SCIMRepresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaAttributeModel_ParentId",
                table: "SCIMSchemaAttributeModel",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaAttributeModel_SchemaId",
                table: "SCIMSchemaAttributeModel",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaExtensionModel_SCIMSchemaModelId",
                table: "SCIMSchemaExtensionModel",
                column: "SCIMSchemaModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentId",
                table: "SCIMRepresentationAttributeLst",
                column: "ParentId",
                principalTable: "SCIMRepresentationAttributeLst",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_RepresentationId",
                table: "SCIMRepresentationAttributeLst",
                column: "RepresentationId",
                principalTable: "SCIMRepresentationLst",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttributeModel_SchemaAttributeId",
                table: "SCIMRepresentationAttributeLst",
                column: "SchemaAttributeId",
                principalTable: "SCIMSchemaAttributeModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
