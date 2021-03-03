using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SCIMAttributeMappingLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    ParentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    Schema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    SCIMSchemaModelId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationAttributeLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SchemaAttributeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RepresentationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationAttributeLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationAttributeLst_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SCIMRepresentationAttributeLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMRepresentationLst_RepresentationId",
                        column: x => x.RepresentationId,
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttributeLst_SCIMSchemaAttributeModel_SchemaAttributeId",
                        column: x => x.SchemaAttributeId,
                        principalTable: "SCIMSchemaAttributeModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationAttributeValueLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ValueString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueInteger = table.Column<int>(type: "int", nullable: true),
                    ValueBoolean = table.Column<bool>(type: "bit", nullable: true),
                    ValueDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValueDecimal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValueByte = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ValueReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SCIMRepresentationAttributeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_ParentId",
                table: "SCIMRepresentationAttributeLst",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_RepresentationId",
                table: "SCIMRepresentationAttributeLst",
                column: "RepresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttributeLst_SchemaAttributeId",
                table: "SCIMRepresentationAttributeLst",
                column: "SchemaAttributeId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SCIMAttributeMappingLst");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationAttributeValueLst");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationSchemaLst");

            migrationBuilder.DropTable(
                name: "SCIMSchemaExtensionModel");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationAttributeLst");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationLst");

            migrationBuilder.DropTable(
                name: "SCIMSchemaAttributeModel");

            migrationBuilder.DropTable(
                name: "SCIMSchemaLst");
        }
    }
}
