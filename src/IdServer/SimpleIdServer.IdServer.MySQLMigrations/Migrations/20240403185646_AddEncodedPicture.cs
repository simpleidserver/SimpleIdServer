using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.MySQLMigrations
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
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PresentationDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublicId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Purpose = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RealmName = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PresentationDefinitionInputDescriptor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublicId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Purpose = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PresentationDefinitionId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionInputDescriptor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionInputDescriptor_PresentationDefinition~",
                        column: x => x.PresentationDefinitionId,
                        principalTable: "PresentationDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PresentationDefinitionFormat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Format = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProofType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PresentationDefinitionInputDescriptorId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionFormat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionFormat_PresentationDefinitionInputDesc~",
                        column: x => x.PresentationDefinitionInputDescriptorId,
                        principalTable: "PresentationDefinitionInputDescriptor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PresentationDefinitionInputDescriptorConstraint",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Path = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Filter = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PresentationDefinitionInputDescriptorId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionInputDescriptorConstraint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionInputDescriptorConstraint_Presentation~",
                        column: x => x.PresentationDefinitionInputDescriptorId,
                        principalTable: "PresentationDefinitionInputDescriptor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitionFormat_PresentationDefinitionInputDesc~",
                table: "PresentationDefinitionFormat",
                column: "PresentationDefinitionInputDescriptorId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitionInputDescriptor_PresentationDefinition~",
                table: "PresentationDefinitionInputDescriptor",
                column: "PresentationDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitionInputDescriptorConstraint_Presentation~",
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
