using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqliteMigrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCredentialTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CredentialOffers");

            migrationBuilder.DropTable(
                name: "CredentialTemplateClaimMapper");

            migrationBuilder.DropTable(
                name: "CredentialTemplateDisplay");

            migrationBuilder.DropTable(
                name: "CredentialTemplateParameter");

            migrationBuilder.DropTable(
                name: "CredentialTemplateRealm");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "BaseCredentialTemplate");

            migrationBuilder.DropColumn(
                name: "Did",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DidPrivateHex",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserPinRequired",
                table: "Clients",
                newName: "IsTransactionCodeRequired");

            migrationBuilder.AddColumn<double>(
                name: "PreAuthCodeExpirationTimeInSeconds",
                table: "Clients",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreAuthCodeExpirationTimeInSeconds",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "IsTransactionCodeRequired",
                table: "Clients",
                newName: "UserPinRequired");

            migrationBuilder.AddColumn<string>(
                name: "Did",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DidPrivateHex",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaseCredentialTemplate",
                columns: table => new
                {
                    TechnicalId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    Format = table.Column<string>(type: "TEXT", nullable: true),
                    Id = table.Column<string>(type: "TEXT", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseCredentialTemplate", x => x.TechnicalId);
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ContractAdr = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PrivateAccountKey = table.Column<string>(type: "TEXT", nullable: false),
                    RpcUrl = table.Column<string>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "CredentialOffers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CredentialTemplateId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CredIssuerState = table.Column<string>(type: "TEXT", nullable: true),
                    CredentialNames = table.Column<string>(type: "TEXT", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Pin = table.Column<string>(type: "TEXT", nullable: true),
                    PreAuthorizedCode = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialOffers_BaseCredentialTemplate_CredentialTemplateId",
                        column: x => x.CredentialTemplateId,
                        principalTable: "BaseCredentialTemplate",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CredentialOffers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialTemplateClaimMapper",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CredentialTemplateId = table.Column<string>(type: "TEXT", nullable: false),
                    IsMultiValued = table.Column<bool>(type: "INTEGER", nullable: false),
                    MapperType = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceUserAttribute = table.Column<string>(type: "TEXT", nullable: true),
                    SourceUserProperty = table.Column<string>(type: "TEXT", nullable: true),
                    TargetClaimPath = table.Column<string>(type: "TEXT", nullable: false),
                    TokenClaimJsonType = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialTemplateClaimMapper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialTemplateClaimMapper_BaseCredentialTemplate_CredentialTemplateId",
                        column: x => x.CredentialTemplateId,
                        principalTable: "BaseCredentialTemplate",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialTemplateDisplay",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CredentialTemplateId = table.Column<string>(type: "TEXT", nullable: true),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Locale = table.Column<string>(type: "TEXT", nullable: true),
                    LogoAltText = table.Column<string>(type: "TEXT", nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    TextColor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialTemplateDisplay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialTemplateDisplay_BaseCredentialTemplate_CredentialTemplateId",
                        column: x => x.CredentialTemplateId,
                        principalTable: "BaseCredentialTemplate",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialTemplateParameter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CredentialTemplateId = table.Column<string>(type: "TEXT", nullable: true),
                    IsArray = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ParameterType = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialTemplateParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialTemplateParameter_BaseCredentialTemplate_CredentialTemplateId",
                        column: x => x.CredentialTemplateId,
                        principalTable: "BaseCredentialTemplate",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialTemplateRealm",
                columns: table => new
                {
                    CredentialTemplatesTechnicalId = table.Column<string>(type: "TEXT", nullable: false),
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialTemplateRealm", x => new { x.CredentialTemplatesTechnicalId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_CredentialTemplateRealm_BaseCredentialTemplate_CredentialTemplatesTechnicalId",
                        column: x => x.CredentialTemplatesTechnicalId,
                        principalTable: "BaseCredentialTemplate",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CredentialTemplateRealm_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CredentialOffers_CredentialTemplateId",
                table: "CredentialOffers",
                column: "CredentialTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CredentialOffers_UserId",
                table: "CredentialOffers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CredentialTemplateClaimMapper_CredentialTemplateId",
                table: "CredentialTemplateClaimMapper",
                column: "CredentialTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CredentialTemplateDisplay_CredentialTemplateId",
                table: "CredentialTemplateDisplay",
                column: "CredentialTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CredentialTemplateParameter_CredentialTemplateId",
                table: "CredentialTemplateParameter",
                column: "CredentialTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CredentialTemplateRealm_RealmsName",
                table: "CredentialTemplateRealm",
                column: "RealmsName");
        }
    }
}
