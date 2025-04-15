using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormBuilder.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormStyle");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Workflows",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Elements = table.Column<string>(type: "text", nullable: false),
                    Windows = table.Column<string>(type: "text", nullable: false),
                    Styles = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateStyle",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<int>(type: "integer", nullable: false),
                    TemplateId = table.Column<string>(type: "text", nullable: true),
                    TemplateId1 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateStyle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateStyle_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TemplateStyle_Templates_TemplateId1",
                        column: x => x.TemplateId1,
                        principalTable: "Templates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateStyle_TemplateId",
                table: "TemplateStyle",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateStyle_TemplateId1",
                table: "TemplateStyle",
                column: "TemplateId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateStyle");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Workflows");

            migrationBuilder.CreateTable(
                name: "FormStyle",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    FormId = table.Column<string>(type: "text", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_FormStyle_FormId",
                table: "FormStyle",
                column: "FormId");
        }
    }
}
