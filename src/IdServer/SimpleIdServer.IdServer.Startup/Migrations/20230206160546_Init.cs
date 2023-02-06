using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.Startup.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Acrs",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthenticationMethodReferences = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acrs", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ApiResources",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResources", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HandlerFullQualifiedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionsFullQualifiedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerializedOptions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BCAuthorizeLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationEdp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interval = table.Column<int>(type: "int", nullable: true),
                    LastStatus = table.Column<int>(type: "int", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectionSentDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextFetchTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCAuthorizeLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClaimProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationAccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrantTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RedirectionUrls = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenEndPointAuthMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JwksUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contacts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoftwareId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoftwareVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSubjectDN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSanDNS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSanURI = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSanIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TlsClientAuthSanEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientSecretExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TokenExpirationTimeInSeconds = table.Column<double>(type: "float", nullable: true),
                    RefreshTokenExpirationTimeInSeconds = table.Column<double>(type: "float", nullable: true),
                    TokenSignedResponseAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenEncryptedResponseAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenEncryptedResponseEnc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredTokenProfile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestObjectSigningAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestObjectEncryptionAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestObjectEncryptionEnc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PairWiseIdentifierSalt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectorIdentifierUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdTokenSignedResponseAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdTokenEncryptedResponseAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdTokenEncryptedResponseEnc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BCTokenDeliveryMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BCClientNotificationEndpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BCAuthenticationRequestSigningAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserInfoSignedResponseAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserInfoEncryptedResponseAlg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserInfoEncryptedResponseEnc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BCUserCodeParameter = table.Column<bool>(type: "bit", nullable: false),
                    FrontChannelLogoutUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(type: "bit", nullable: false),
                    BackChannelLogoutSessionRequired = table.Column<bool>(type: "bit", nullable: false),
                    DefaultMaxAge = table.Column<double>(type: "float", nullable: true),
                    TlsClientCertificateBoundAccessToken = table.Column<bool>(type: "bit", nullable: false),
                    BackChannelLogoutUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitiateLoginUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequireAuthTime = table.Column<bool>(type: "bit", nullable: false),
                    IsConsentDisabled = table.Column<bool>(type: "bit", nullable: false),
                    IsResourceParameterRequired = table.Column<bool>(type: "bit", nullable: false),
                    AuthReqIdExpirationTimeInSeconds = table.Column<int>(type: "int", nullable: false),
                    SerializedParameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultAcrValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BCIntervalSeconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "Grants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Claims = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
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
                    table.PrimaryKey("PK_Scopes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    PkID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRegistrationAccessToken = table.Column<bool>(type: "bit", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.PkID);
                });

            migrationBuilder.CreateTable(
                name: "UMAPermissionTicket",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAPermissionTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UmaResources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IconUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmaResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    DeviceRegistrationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BCAuthorizeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BCAuthorizeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCAuthorizeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BCAuthorizeHistory_BCAuthorizeLst_BCAuthorizeId",
                        column: x => x.BCAuthorizeId,
                        principalTable: "BCAuthorizeLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientJsonWebKey",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerializedJsonWebKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientJsonWebKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientJsonWebKey_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizedScope",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resources = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GrantId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedScope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizedScope_Grants_GrantId",
                        column: x => x.GrantId,
                        principalTable: "Grants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiResourceScope",
                columns: table => new
                {
                    ApiResourcesName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScopesName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResourceScope", x => new { x.ApiResourcesName, x.ScopesName });
                    table.ForeignKey(
                        name: "FK_ApiResourceScope_ApiResources_ApiResourcesName",
                        column: x => x.ApiResourcesName,
                        principalTable: "ApiResources",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiResourceScope_Scopes_ScopesName",
                        column: x => x.ScopesName,
                        principalTable: "Scopes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientScope",
                columns: table => new
                {
                    ClientsClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScopesName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientScope", x => new { x.ClientsClientId, x.ScopesName });
                    table.ForeignKey(
                        name: "FK_ClientScope_Clients_ClientsClientId",
                        column: x => x.ClientsClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientScope_Scopes_ScopesName",
                        column: x => x.ScopesName,
                        principalTable: "Scopes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScopeClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsExposed = table.Column<bool>(type: "bit", nullable: false),
                    ScopeName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScopeClaim_Scopes_ScopeName",
                        column: x => x.ScopeName,
                        principalTable: "Scopes",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "UMAPermissionTicketRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UMAPermissionTicketId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAPermissionTicketRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UMAPermissionTicketRecord_UMAPermissionTicket_UMAPermissionTicketId",
                        column: x => x.UMAPermissionTicketId,
                        principalTable: "UMAPermissionTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UmaPendingRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requester = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmaPendingRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UmaPendingRequest_UmaResources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "UmaResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UMAResourcePermission",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UMAResourceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAResourcePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UMAResourcePermission_UmaResources_UMAResourceId",
                        column: x => x.UMAResourceId,
                        principalTable: "UmaResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Claims = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScopeName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consent_Scopes_ScopeName",
                        column: x => x.ScopeName,
                        principalTable: "Scopes",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Consent_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredential",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CredentialType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OTPAlg = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OTPCounter = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredential", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCredential_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDevice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerializedOptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevice_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExternalAuthProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scheme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExternalAuthProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExternalAuthProvider_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_UserSession_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationUMAResource",
                columns: table => new
                {
                    TranslationsId = table.Column<int>(type: "int", nullable: false),
                    UMAResourceId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationUMAResource", x => new { x.TranslationsId, x.UMAResourceId });
                    table.ForeignKey(
                        name: "FK_TranslationUMAResource_Translations_TranslationsId",
                        column: x => x.TranslationsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslationUMAResource_UmaResources_UMAResourceId",
                        column: x => x.UMAResourceId,
                        principalTable: "UmaResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UMAResourcePermissionClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScope_ScopesName",
                table: "ApiResourceScope",
                column: "ScopesName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedScope_GrantId",
                table: "AuthorizedScope",
                column: "GrantId");

            migrationBuilder.CreateIndex(
                name: "IX_BCAuthorizeHistory_BCAuthorizeId",
                table: "BCAuthorizeHistory",
                column: "BCAuthorizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientJsonWebKey_ClientId",
                table: "ClientJsonWebKey",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientScope_ScopesName",
                table: "ClientScope",
                column: "ScopesName");

            migrationBuilder.CreateIndex(
                name: "IX_Consent_ScopeName",
                table: "Consent",
                column: "ScopeName");

            migrationBuilder.CreateIndex(
                name: "IX_Consent_UserId",
                table: "Consent",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeClaim_ScopeName",
                table: "ScopeClaim",
                column: "ScopeName");

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_Name",
                table: "Scopes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_ClientId",
                table: "Translations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationUMAResource_UMAResourceId",
                table: "TranslationUMAResource",
                column: "UMAResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UmaPendingRequest_ResourceId",
                table: "UmaPendingRequest",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UMAPermissionTicketRecord_UMAPermissionTicketId",
                table: "UMAPermissionTicketRecord",
                column: "UMAPermissionTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_UMAResourcePermission_UMAResourceId",
                table: "UMAResourcePermission",
                column: "UMAResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_UMAResourcePermissionClaim_UMAResourcePermissionId",
                table: "UMAResourcePermissionClaim",
                column: "UMAResourcePermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredential_UserId",
                table: "UserCredential",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevice_UserId",
                table: "UserDevice",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Acrs");

            migrationBuilder.DropTable(
                name: "ApiResourceScope");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviders");

            migrationBuilder.DropTable(
                name: "AuthorizedScope");

            migrationBuilder.DropTable(
                name: "BCAuthorizeHistory");

            migrationBuilder.DropTable(
                name: "ClaimProviders");

            migrationBuilder.DropTable(
                name: "ClientJsonWebKey");

            migrationBuilder.DropTable(
                name: "ClientScope");

            migrationBuilder.DropTable(
                name: "Consent");

            migrationBuilder.DropTable(
                name: "ScopeClaim");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "TranslationUMAResource");

            migrationBuilder.DropTable(
                name: "UmaPendingRequest");

            migrationBuilder.DropTable(
                name: "UMAPermissionTicketRecord");

            migrationBuilder.DropTable(
                name: "UMAResourcePermissionClaim");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserCredential");

            migrationBuilder.DropTable(
                name: "UserDevice");

            migrationBuilder.DropTable(
                name: "UserExternalAuthProvider");

            migrationBuilder.DropTable(
                name: "UserSession");

            migrationBuilder.DropTable(
                name: "ApiResources");

            migrationBuilder.DropTable(
                name: "Grants");

            migrationBuilder.DropTable(
                name: "BCAuthorizeLst");

            migrationBuilder.DropTable(
                name: "Scopes");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "UMAPermissionTicket");

            migrationBuilder.DropTable(
                name: "UMAResourcePermission");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "UmaResources");
        }
    }
}
