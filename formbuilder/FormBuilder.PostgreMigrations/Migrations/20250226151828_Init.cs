using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Realm = table.Column<string>(type: "text", nullable: true),
                    ActAsStep = table.Column<bool>(type: "boolean", nullable: false),
                    Elements = table.Column<string>(type: "text", nullable: false),
                    CorrelationId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormStyle",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FormId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormStyle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormStyle_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowLink",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    SourceStepId = table.Column<string>(type: "text", nullable: false),
                    TargetStepId = table.Column<string>(type: "text", nullable: true),
                    ActionType = table.Column<string>(type: "text", nullable: true),
                    ActionParameter = table.Column<string>(type: "text", nullable: true),
                    IsMainLink = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    WorkflowRecordId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowLink_Workflows_WorkflowRecordId",
                        column: x => x.WorkflowRecordId,
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStep",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FormRecordCorrelationId = table.Column<string>(type: "text", nullable: false),
                    WorkflowRecordId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStep_Workflows_WorkflowRecordId",
                        column: x => x.WorkflowRecordId,
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormStyle_FormId",
                table: "FormStyle",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowLink_WorkflowRecordId",
                table: "WorkflowLink",
                column: "WorkflowRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStep_WorkflowRecordId",
                table: "WorkflowStep",
                column: "WorkflowRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormStyle");

            migrationBuilder.DropTable(
                name: "WorkflowLink");

            migrationBuilder.DropTable(
                name: "WorkflowStep");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "Workflows");
        }
    }
}
