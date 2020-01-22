using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.Migrations.SqlServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SCIMRepresentationLst",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ExternalId = table.Column<string>(nullable: true),
                    ResourceType = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaLst",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SCIMRepresentationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaLst_SCIMRepresentationLst_SCIMRepresentationId",
                        column: x => x.SCIMRepresentationId,
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SCIMSchemaAttribute",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    MultiValued = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    CanonicalValues = table.Column<string>(nullable: true),
                    CaseExact = table.Column<bool>(nullable: false),
                    Mutability = table.Column<int>(nullable: false),
                    Returned = table.Column<int>(nullable: false),
                    Uniqueness = table.Column<int>(nullable: false),
                    ReferenceTypes = table.Column<string>(nullable: true),
                    DefaultValueString = table.Column<string>(nullable: true),
                    DefaultValueInt = table.Column<string>(nullable: true),
                    SCIMSchemaAttributeId = table.Column<string>(nullable: true),
                    SCIMSchemaId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMSchemaAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaAttribute_SCIMSchemaAttribute_SCIMSchemaAttributeId",
                        column: x => x.SCIMSchemaAttributeId,
                        principalTable: "SCIMSchemaAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SCIMSchemaAttribute_SCIMSchemaLst_SCIMSchemaId",
                        column: x => x.SCIMSchemaId,
                        principalTable: "SCIMSchemaLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationAttribute",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ValuesString = table.Column<string>(nullable: true),
                    ValuesBoolean = table.Column<string>(nullable: true),
                    ValuesInteger = table.Column<string>(nullable: true),
                    ValuesDateTime = table.Column<string>(nullable: true),
                    ValuesReference = table.Column<string>(nullable: true),
                    ParentId = table.Column<string>(nullable: true),
                    SchemaAttributeId = table.Column<string>(nullable: true),
                    SCIMRepresentationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttribute_SCIMRepresentationAttribute_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SCIMRepresentationAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttribute_SCIMRepresentationLst_SCIMRepresentationId",
                        column: x => x.SCIMRepresentationId,
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationAttribute_SCIMSchemaAttribute_SchemaAttributeId",
                        column: x => x.SchemaAttributeId,
                        principalTable: "SCIMSchemaAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttribute_ParentId",
                table: "SCIMRepresentationAttribute",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttribute_SCIMRepresentationId",
                table: "SCIMRepresentationAttribute",
                column: "SCIMRepresentationId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationAttribute_SchemaAttributeId",
                table: "SCIMRepresentationAttribute",
                column: "SchemaAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaAttribute_SCIMSchemaAttributeId",
                table: "SCIMSchemaAttribute",
                column: "SCIMSchemaAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaAttribute_SCIMSchemaId",
                table: "SCIMSchemaAttribute",
                column: "SCIMSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_SCIMSchemaLst_SCIMRepresentationId",
                table: "SCIMSchemaLst",
                column: "SCIMRepresentationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SCIMRepresentationAttribute");

            migrationBuilder.DropTable(
                name: "SCIMSchemaAttribute");

            migrationBuilder.DropTable(
                name: "SCIMSchemaLst");

            migrationBuilder.DropTable(
                name: "SCIMRepresentationLst");
        }
    }
}
