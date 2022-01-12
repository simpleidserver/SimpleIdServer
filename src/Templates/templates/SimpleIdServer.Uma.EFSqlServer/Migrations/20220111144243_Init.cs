using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleIdServer.Uma.EFSqlServer.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OAuthClients",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientSecretExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TokenSignedResponseAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenEncryptedResponseAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenEncryptedResponseEnc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenEndPointAuthMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrantTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedirectionUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JwksUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenExpirationTimeInSeconds = table.Column<double>(type: "float", nullable: false),
                    RefreshTokenExpirationTimeInSeconds = table.Column<double>(type: "float", nullable: false),
                    PreferredTokenProfile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contacts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientCertificateBoundAccessToken = table.Column<bool>(type: "bit", nullable: false),
                    TlsClientAuthSubjectDN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSanDNS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSanURI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSanIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSanEmail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthClients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "OAuthScopes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsStandardScope = table.Column<bool>(type: "bit", nullable: false),
                    IsExposedInConfigurationEdp = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthScopes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "OAuthTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthTranslation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    PkID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRegistrationAccessToken = table.Column<bool>(type: "bit", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.PkID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeviceRegistrationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OTPKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OTPCounter = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JsonWebKeys",
                columns: table => new
                {
                    Kid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Kty = table.Column<int>(type: "int", nullable: false),
                    Use = table.Column<int>(type: "int", nullable: false),
                    Alg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RotationJWKId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OAuthClientClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonWebKeys", x => x.Kid);
                    table.ForeignKey(
                        name: "FK_JsonWebKeys_OAuthClients_OAuthClientClientId",
                        column: x => x.OAuthClientClientId,
                        principalTable: "OAuthClients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OAuthClientOAuthScope",
                columns: table => new
                {
                    ClientsClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OAuthAllowedScopesName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthClientOAuthScope", x => new { x.ClientsClientId, x.OAuthAllowedScopesName });
                    table.ForeignKey(
                        name: "FK_OAuthClientOAuthScope_OAuthClients_ClientsClientId",
                        column: x => x.ClientsClientId,
                        principalTable: "OAuthClients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OAuthClientOAuthScope_OAuthScopes_OAuthAllowedScopesName",
                        column: x => x.OAuthAllowedScopesName,
                        principalTable: "OAuthScopes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OAuthScopeClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsExposed = table.Column<bool>(type: "bit", nullable: false),
                    OAuthScopeName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthScopeClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthScopeClaim_OAuthScopes_OAuthScopeName",
                        column: x => x.OAuthScopeName,
                        principalTable: "OAuthScopes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OAuthClientTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TranslationId = table.Column<int>(type: "int", nullable: true),
                    OAuthClientClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthClientTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthClientTranslation_OAuthClients_OAuthClientClientId",
                        column: x => x.OAuthClientClientId,
                        principalTable: "OAuthClients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OAuthClientTranslation_OAuthTranslation_TranslationId",
                        column: x => x.TranslationId,
                        principalTable: "OAuthTranslation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PendingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requester = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingRequests_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UMAResourcePermission",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UMAResourceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAResourcePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UMAResourcePermission_Resources_UMAResourceId",
                        column: x => x.UMAResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UMAResourceTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TranslationId = table.Column<int>(type: "int", nullable: true),
                    UMAResourceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAResourceTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UMAResourceTranslation_OAuthTranslation_TranslationId",
                        column: x => x.TranslationId,
                        principalTable: "OAuthTranslation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UMAResourceTranslation_Resources_UMAResourceId",
                        column: x => x.UMAResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OAuthConsent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Claims = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OAuthUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthConsent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthConsent_User_OAuthUserId",
                        column: x => x.OAuthUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredential",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CredentialType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredential", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredential_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExternalAuthProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scheme = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExternalAuthProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExternalAuthProvider_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSession",
                columns: table => new
                {
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AuthenticationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_UserSession_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JsonWebKeyKeyOperation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Operation = table.Column<int>(type: "int", nullable: false),
                    JsonWebKeyKid = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonWebKeyKeyOperation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JsonWebKeyKeyOperation_JsonWebKeys_JsonWebKeyKid",
                        column: x => x.JsonWebKeyKid,
                        principalTable: "JsonWebKeys",
                        principalColumn: "Kid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UMAResourcePermissionClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UMAResourcePermissionId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAResourcePermissionClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UMAResourcePermissionClaim_UMAResourcePermission_UMAResourcePermissionId",
                        column: x => x.UMAResourcePermissionId,
                        principalTable: "UMAResourcePermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OAuthConsentOAuthScope",
                columns: table => new
                {
                    ConsentsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScopesName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthConsentOAuthScope", x => new { x.ConsentsId, x.ScopesName });
                    table.ForeignKey(
                        name: "FK_OAuthConsentOAuthScope_OAuthConsent_ConsentsId",
                        column: x => x.ConsentsId,
                        principalTable: "OAuthConsent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OAuthConsentOAuthScope_OAuthScopes_ScopesName",
                        column: x => x.ScopesName,
                        principalTable: "OAuthScopes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JsonWebKeyKeyOperation_JsonWebKeyKid",
                table: "JsonWebKeyKeyOperation",
                column: "JsonWebKeyKid");

            migrationBuilder.CreateIndex(
                name: "IX_JsonWebKeys_OAuthClientClientId",
                table: "JsonWebKeys",
                column: "OAuthClientClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthClientOAuthScope_OAuthAllowedScopesName",
                table: "OAuthClientOAuthScope",
                column: "OAuthAllowedScopesName");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthClientTranslation_OAuthClientClientId",
                table: "OAuthClientTranslation",
                column: "OAuthClientClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthClientTranslation_TranslationId",
                table: "OAuthClientTranslation",
                column: "TranslationId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthConsent_OAuthUserId",
                table: "OAuthConsent",
                column: "OAuthUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthConsentOAuthScope_ScopesName",
                table: "OAuthConsentOAuthScope",
                column: "ScopesName");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthScopeClaim_OAuthScopeName",
                table: "OAuthScopeClaim",
                column: "OAuthScopeName");

            migrationBuilder.CreateIndex(
                name: "IX_OAuthScopes_Name",
                table: "OAuthScopes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PendingRequests_ResourceId",
                table: "PendingRequests",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UMAResourcePermission_UMAResourceId",
                table: "UMAResourcePermission",
                column: "UMAResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UMAResourcePermissionClaim_UMAResourcePermissionId",
                table: "UMAResourcePermissionClaim",
                column: "UMAResourcePermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UMAResourceTranslation_TranslationId",
                table: "UMAResourceTranslation",
                column: "TranslationId");

            migrationBuilder.CreateIndex(
                name: "IX_UMAResourceTranslation_UMAResourceId",
                table: "UMAResourceTranslation",
                column: "UMAResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredential_UserId",
                table: "UserCredential",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExternalAuthProvider_UserId",
                table: "UserExternalAuthProvider",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId",
                table: "UserSession",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JsonWebKeyKeyOperation");

            migrationBuilder.DropTable(
                name: "OAuthClientOAuthScope");

            migrationBuilder.DropTable(
                name: "OAuthClientTranslation");

            migrationBuilder.DropTable(
                name: "OAuthConsentOAuthScope");

            migrationBuilder.DropTable(
                name: "OAuthScopeClaim");

            migrationBuilder.DropTable(
                name: "PendingRequests");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UMAResourcePermissionClaim");

            migrationBuilder.DropTable(
                name: "UMAResourceTranslation");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserCredential");

            migrationBuilder.DropTable(
                name: "UserExternalAuthProvider");

            migrationBuilder.DropTable(
                name: "UserSession");

            migrationBuilder.DropTable(
                name: "JsonWebKeys");

            migrationBuilder.DropTable(
                name: "OAuthConsent");

            migrationBuilder.DropTable(
                name: "OAuthScopes");

            migrationBuilder.DropTable(
                name: "UMAResourcePermission");

            migrationBuilder.DropTable(
                name: "OAuthTranslation");

            migrationBuilder.DropTable(
                name: "OAuthClients");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Resources");
        }
    }
}
