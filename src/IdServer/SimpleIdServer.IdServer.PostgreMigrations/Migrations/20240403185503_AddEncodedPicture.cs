using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddEncodedPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncodedPicture",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PresentationDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    RealmName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitions_Realms_RealmName",
                        column: x => x.RealmName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresentationDefinitionInputDescriptor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    PresentationDefinitionId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionInputDescriptor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionInputDescriptor_PresentationDefinitio~",
                        column: x => x.PresentationDefinitionId,
                        principalTable: "PresentationDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresentationDefinitionFormat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Format = table.Column<string>(type: "text", nullable: false),
                    ProofType = table.Column<string>(type: "text", nullable: false),
                    PresentationDefinitionInputDescriptorId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionFormat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionFormat_PresentationDefinitionInputDes~",
                        column: x => x.PresentationDefinitionInputDescriptorId,
                        principalTable: "PresentationDefinitionInputDescriptor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresentationDefinitionInputDescriptorConstraint",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Filter = table.Column<string>(type: "text", nullable: false),
                    PresentationDefinitionInputDescriptorId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionInputDescriptorConstraint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionInputDescriptorConstraint_Presentatio~",
                        column: x => x.PresentationDefinitionInputDescriptorId,
                        principalTable: "PresentationDefinitionInputDescriptor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitionFormat_PresentationDefinitionInputDes~",
                table: "PresentationDefinitionFormat",
                column: "PresentationDefinitionInputDescriptorId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitionInputDescriptor_PresentationDefinitio~",
                table: "PresentationDefinitionInputDescriptor",
                column: "PresentationDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitionInputDescriptorConstraint_Presentatio~",
                table: "PresentationDefinitionInputDescriptorConstraint",
                column: "PresentationDefinitionInputDescriptorId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitions_RealmName",
                table: "PresentationDefinitions",
                column: "RealmName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PresentationDefinitionFormat");

            migrationBuilder.DropTable(
                name: "PresentationDefinitionInputDescriptorConstraint");

            migrationBuilder.DropTable(
                name: "PresentationDefinitionInputDescriptor");

            migrationBuilder.DropTable(
                name: "PresentationDefinitions");

            migrationBuilder.DropColumn(
                name: "EncodedPicture",
                table: "Users");
        }
    }
}
