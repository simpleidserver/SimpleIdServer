using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Scim.SqlServer.Startup.Migrations
{
    public partial class AddIndirectReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "SCIMAttributeMappingLst",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SCIMRepresentationIndirectReference",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NbReferences = table.Column<int>(type: "int", nullable: false),
                    TargetReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetAttributeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SCIMRepresentationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCIMRepresentationIndirectReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SCIMRepresentationIndirectReference_SCIMRepresentationLst_SCIMRepresentationId",
                        column: x => x.SCIMRepresentationId,
                        principalTable: "SCIMRepresentationLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SCIMRepresentationIndirectReference_SCIMRepresentationId",
                table: "SCIMRepresentationIndirectReference",
                column: "SCIMRepresentationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SCIMRepresentationIndirectReference");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "SCIMAttributeMappingLst");
        }
    }
}
