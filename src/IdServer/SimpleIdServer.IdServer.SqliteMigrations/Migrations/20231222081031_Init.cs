using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.SqliteMigrations.Migrations
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
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    AuthenticationMethodReferences = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acrs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiResources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Audience = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    EventName = table.Column<string>(type: "TEXT", nullable: false),
                    Realm = table.Column<string>(type: "TEXT", nullable: false),
                    IsError = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    RequestJSON = table.Column<string>(type: "TEXT", nullable: true),
                    RedirectUrl = table.Column<string>(type: "TEXT", nullable: true),
                    AuthMethod = table.Column<string>(type: "TEXT", nullable: true),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false),
                    Claims = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderDefinitions",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    HandlerFullQualifiedName = table.Column<string>(type: "TEXT", nullable: true),
                    OptionsFullQualifiedName = table.Column<string>(type: "TEXT", nullable: true),
                    OptionsName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderDefinitions", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "BaseCredentialTemplate",
                columns: table => new
                {
                    TechnicalId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<string>(type: "TEXT", nullable: true),
                    Format = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseCredentialTemplate", x => x.TechnicalId);
                });

            migrationBuilder.CreateTable(
                name: "BCAuthorizeLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationToken = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationMode = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationEdp = table.Column<string>(type: "TEXT", nullable: true),
                    Interval = table.Column<int>(type: "INTEGER", nullable: true),
                    LastStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RejectionSentDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextFetchTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Realm = table.Column<string>(type: "TEXT", nullable: false),
                    SerializedAuthorizationDetails = table.Column<string>(type: "TEXT", nullable: true),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCAuthorizeLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateAuthorities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SubjectName = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    StoreLocation = table.Column<int>(type: "INTEGER", nullable: true),
                    StoreName = table.Column<int>(type: "INTEGER", nullable: true),
                    FindType = table.Column<int>(type: "INTEGER", nullable: true),
                    FindValue = table.Column<string>(type: "TEXT", nullable: true),
                    PublicKey = table.Column<string>(type: "TEXT", nullable: true),
                    PrivateKey = table.Column<string>(type: "TEXT", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateAuthorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClaimProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderType = table.Column<string>(type: "TEXT", nullable: false),
                    ConnectionString = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    ClientSecret = table.Column<string>(type: "TEXT", nullable: false),
                    RegistrationAccessToken = table.Column<string>(type: "TEXT", nullable: true),
                    GrantTypes = table.Column<string>(type: "TEXT", nullable: false),
                    RedirectionUrls = table.Column<string>(type: "TEXT", nullable: false),
                    TokenEndPointAuthMethod = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseTypes = table.Column<string>(type: "TEXT", nullable: false),
                    JwksUri = table.Column<string>(type: "TEXT", nullable: true),
                    Contacts = table.Column<string>(type: "TEXT", nullable: false),
                    SoftwareId = table.Column<string>(type: "TEXT", nullable: true),
                    SoftwareVersion = table.Column<string>(type: "TEXT", nullable: true),
                    TlsClientAuthSubjectDN = table.Column<string>(type: "TEXT", nullable: true),
                    TlsClientAuthSanDNS = table.Column<string>(type: "TEXT", nullable: true),
                    TlsClientAuthSanURI = table.Column<string>(type: "TEXT", nullable: true),
                    TlsClientAuthSanIP = table.Column<string>(type: "TEXT", nullable: true),
                    TlsClientAuthSanEmail = table.Column<string>(type: "TEXT", nullable: true),
                    ClientSecretExpirationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsTokenExchangeEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    TokenExchangeType = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TokenExpirationTimeInSeconds = table.Column<double>(type: "REAL", nullable: true),
                    CNonceExpirationTimeInSeconds = table.Column<double>(type: "REAL", nullable: true),
                    RefreshTokenExpirationTimeInSeconds = table.Column<double>(type: "REAL", nullable: true),
                    TokenSignedResponseAlg = table.Column<string>(type: "TEXT", nullable: true),
                    TokenEncryptedResponseAlg = table.Column<string>(type: "TEXT", nullable: true),
                    TokenEncryptedResponseEnc = table.Column<string>(type: "TEXT", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "TEXT", nullable: false),
                    RedirectToRevokeSessionUI = table.Column<bool>(type: "INTEGER", nullable: false),
                    PreferredTokenProfile = table.Column<string>(type: "TEXT", nullable: true),
                    RequestObjectSigningAlg = table.Column<string>(type: "TEXT", nullable: true),
                    RequestObjectEncryptionAlg = table.Column<string>(type: "TEXT", nullable: true),
                    RequestObjectEncryptionEnc = table.Column<string>(type: "TEXT", nullable: true),
                    SubjectType = table.Column<string>(type: "TEXT", nullable: true),
                    PairWiseIdentifierSalt = table.Column<string>(type: "TEXT", nullable: true),
                    SectorIdentifierUri = table.Column<string>(type: "TEXT", nullable: true),
                    IdTokenSignedResponseAlg = table.Column<string>(type: "TEXT", nullable: true),
                    IdTokenEncryptedResponseAlg = table.Column<string>(type: "TEXT", nullable: true),
                    IdTokenEncryptedResponseEnc = table.Column<string>(type: "TEXT", nullable: true),
                    BCTokenDeliveryMode = table.Column<string>(type: "TEXT", nullable: true),
                    BCClientNotificationEndpoint = table.Column<string>(type: "TEXT", nullable: true),
                    BCAuthenticationRequestSigningAlg = table.Column<string>(type: "TEXT", nullable: true),
                    UserInfoSignedResponseAlg = table.Column<string>(type: "TEXT", nullable: true),
                    UserInfoEncryptedResponseAlg = table.Column<string>(type: "TEXT", nullable: true),
                    UserInfoEncryptedResponseEnc = table.Column<string>(type: "TEXT", nullable: true),
                    BCUserCodeParameter = table.Column<bool>(type: "INTEGER", nullable: false),
                    FrontChannelLogoutUri = table.Column<string>(type: "TEXT", nullable: true),
                    CredentialOfferEndpoint = table.Column<string>(type: "TEXT", nullable: true),
                    UserPinRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackChannelLogoutSessionRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultMaxAge = table.Column<double>(type: "REAL", nullable: true),
                    TlsClientCertificateBoundAccessToken = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackChannelLogoutUri = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationType = table.Column<string>(type: "TEXT", nullable: true),
                    InitiateLoginUri = table.Column<string>(type: "TEXT", nullable: true),
                    RequireAuthTime = table.Column<bool>(type: "INTEGER", nullable: false),
                    AuthorizationSignedResponseAlg = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorizationEncryptedResponseAlg = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorizationDataTypes = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorizationEncryptedResponseEnc = table.Column<string>(type: "TEXT", nullable: true),
                    DPOPBoundAccessTokens = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsConsentDisabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsResourceParameterRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    AuthReqIdExpirationTimeInSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientType = table.Column<string>(type: "TEXT", nullable: true),
                    IsDPOPNonceRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    DPOPNonceLifetimeInSeconds = table.Column<double>(type: "REAL", nullable: false),
                    IsRedirectUrlCaseSensitive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SerializedParameters = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultAcrValues = table.Column<string>(type: "TEXT", nullable: false),
                    BCIntervalSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    AccessTokenType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationKeyPairValueRecords",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationKeyPairValueRecords", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FriendlyName = table.Column<string>(type: "TEXT", nullable: true),
                    Xml = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Definitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FullQualifiedName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtractedRepresentations",
                columns: table => new
                {
                    ExternalId = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractedRepresentations", x => x.ExternalId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    FullPath = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ParentGroupId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Groups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningDefinitions",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OptionsName = table.Column<string>(type: "TEXT", nullable: false),
                    OptionsFullQualifiedName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningDefinitions", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RpcUrl = table.Column<string>(type: "TEXT", nullable: false),
                    PrivateAccountKey = table.Column<string>(type: "TEXT", nullable: false),
                    ContractAdr = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Protocol = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsExposedInConfigurationEdp = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerializedFileKeys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    KeyId = table.Column<string>(type: "TEXT", nullable: false),
                    Usage = table.Column<string>(type: "TEXT", nullable: false),
                    Alg = table.Column<string>(type: "TEXT", nullable: false),
                    Enc = table.Column<string>(type: "TEXT", nullable: true),
                    PublicKeyPem = table.Column<string>(type: "TEXT", nullable: true),
                    PrivateKeyPem = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsSymmetric = table.Column<bool>(type: "INTEGER", nullable: false),
                    Key = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerializedFileKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    PkID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Id = table.Column<string>(type: "TEXT", nullable: true),
                    SessionId = table.Column<string>(type: "TEXT", nullable: true),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    TokenType = table.Column<string>(type: "TEXT", nullable: false),
                    AccessTokenType = table.Column<int>(type: "INTEGER", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    OriginalData = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "TEXT", nullable: true),
                    GrantId = table.Column<string>(type: "TEXT", nullable: true),
                    Jkt = table.Column<string>(type: "TEXT", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.PkID);
                });

            migrationBuilder.CreateTable(
                name: "UMAPermissionTicket",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAPermissionTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UmaResources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IconUri = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false),
                    Realm = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmaResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthSchemeProviderDefinitionName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviders_AuthenticationSchemeProviderDefinitions_AuthSchemeProviderDefinitionName",
                        column: x => x.AuthSchemeProviderDefinitionName,
                        principalTable: "AuthenticationSchemeProviderDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialTemplateClaimMapper",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    MapperType = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceUserAttribute = table.Column<string>(type: "TEXT", nullable: true),
                    SourceUserProperty = table.Column<string>(type: "TEXT", nullable: true),
                    TargetClaimPath = table.Column<string>(type: "TEXT", nullable: false),
                    IsMultiValued = table.Column<bool>(type: "INTEGER", nullable: false),
                    TokenClaimJsonType = table.Column<int>(type: "INTEGER", nullable: true),
                    CredentialTemplateId = table.Column<string>(type: "TEXT", nullable: false)
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
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Locale = table.Column<string>(type: "TEXT", nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    LogoAltText = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    TextColor = table.Column<string>(type: "TEXT", nullable: true),
                    CredentialTemplateId = table.Column<string>(type: "TEXT", nullable: true)
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
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    CredentialTemplateId = table.Column<string>(type: "TEXT", nullable: true),
                    ParameterType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArray = table.Column<bool>(type: "INTEGER", nullable: false)
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
                name: "BCAuthorizeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    BCAuthorizeId = table.Column<string>(type: "TEXT", nullable: true)
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
                name: "ClientCertificate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PublicKey = table.Column<string>(type: "TEXT", nullable: false),
                    PrivateKey = table.Column<string>(type: "TEXT", nullable: false),
                    CertificateAuthorityId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCertificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientCertificate_CertificateAuthorities_CertificateAuthorityId",
                        column: x => x.CertificateAuthorityId,
                        principalTable: "CertificateAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientJsonWebKey",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kid = table.Column<string>(type: "TEXT", nullable: false),
                    Alg = table.Column<string>(type: "TEXT", nullable: false),
                    Usage = table.Column<string>(type: "TEXT", nullable: false),
                    KeyType = table.Column<int>(type: "INTEGER", nullable: true),
                    SerializedJsonWebKey = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientJsonWebKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientJsonWebKey_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Language = table.Column<string>(type: "TEXT", nullable: true),
                    ClientId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayCondition = table.Column<string>(type: "TEXT", nullable: true),
                    ConfigurationDefinitionId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecord_Definitions_ConfigurationDefinitionId",
                        column: x => x.ConfigurationDefinitionId,
                        principalTable: "Definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DefinitionName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningLst_IdentityProvisioningDefinitions_DefinitionName",
                        column: x => x.DefinitionName,
                        principalTable: "IdentityProvisioningDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningMappingRule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    From = table.Column<string>(type: "TEXT", nullable: false),
                    MapperType = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetUserAttribute = table.Column<string>(type: "TEXT", nullable: true),
                    TargetUserProperty = table.Column<string>(type: "TEXT", nullable: true),
                    HasMultipleAttribute = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdentityProvisioningDefinitionName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningMappingRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningMappingRule_IdentityProvisioningDefinitions_IdentityProvisioningDefinitionName",
                        column: x => x.IdentityProvisioningDefinitionName,
                        principalTable: "IdentityProvisioningDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiResourceRealm",
                columns: table => new
                {
                    ApiResourcesId = table.Column<string>(type: "TEXT", nullable: false),
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResourceRealm", x => new { x.ApiResourcesId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_ApiResourceRealm_ApiResources_ApiResourcesId",
                        column: x => x.ApiResourcesId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiResourceRealm_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationContextClassReferenceRealm",
                columns: table => new
                {
                    AuthenticationContextClassReferencesId = table.Column<string>(type: "TEXT", nullable: false),
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationContextClassReferenceRealm", x => new { x.AuthenticationContextClassReferencesId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_AuthenticationContextClassReferenceRealm_Acrs_AuthenticationContextClassReferencesId",
                        column: x => x.AuthenticationContextClassReferencesId,
                        principalTable: "Acrs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthenticationContextClassReferenceRealm_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateAuthorityRealm",
                columns: table => new
                {
                    CertificateAuthoritiesId = table.Column<string>(type: "TEXT", nullable: false),
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateAuthorityRealm", x => new { x.CertificateAuthoritiesId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_CertificateAuthorityRealm_CertificateAuthorities_CertificateAuthoritiesId",
                        column: x => x.CertificateAuthoritiesId,
                        principalTable: "CertificateAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateAuthorityRealm_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientRealm",
                columns: table => new
                {
                    ClientsId = table.Column<string>(type: "TEXT", nullable: false),
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRealm", x => new { x.ClientsId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_ClientRealm_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientRealm_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
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

            migrationBuilder.CreateTable(
                name: "GroupRealm",
                columns: table => new
                {
                    GroupsId = table.Column<string>(type: "TEXT", nullable: false),
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRealm", x => new { x.GroupsId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_GroupRealm_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRealm_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportSummaries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NbRepresentations = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RealmName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportSummaries_Realms_RealmName",
                        column: x => x.RealmName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationWorkflows",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RealmName = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Steps = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationWorkflows_Realms_RealmName",
                        column: x => x.RealmName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiResourceScope",
                columns: table => new
                {
                    ApiResourcesId = table.Column<string>(type: "TEXT", nullable: false),
                    ScopesId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResourceScope", x => new { x.ApiResourcesId, x.ScopesId });
                    table.ForeignKey(
                        name: "FK_ApiResourceScope_ApiResources_ApiResourcesId",
                        column: x => x.ApiResourcesId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiResourceScope_Scopes_ScopesId",
                        column: x => x.ScopesId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientScope",
                columns: table => new
                {
                    ClientsId = table.Column<string>(type: "TEXT", nullable: false),
                    ScopesId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientScope", x => new { x.ClientsId, x.ScopesId });
                    table.ForeignKey(
                        name: "FK_ClientScope_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientScope_Scopes_ScopesId",
                        column: x => x.ScopesId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupScope",
                columns: table => new
                {
                    GroupsId = table.Column<string>(type: "TEXT", nullable: false),
                    RolesId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupScope", x => new { x.GroupsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_GroupScope_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupScope_Scopes_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealmScope",
                columns: table => new
                {
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false),
                    ScopesId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealmScope", x => new { x.RealmsName, x.ScopesId });
                    table.ForeignKey(
                        name: "FK_RealmScope_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealmScope_Scopes_ScopesId",
                        column: x => x.ScopesId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScopeClaimMapper",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MapperType = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceUserAttribute = table.Column<string>(type: "TEXT", nullable: true),
                    SourceUserProperty = table.Column<string>(type: "TEXT", nullable: true),
                    TargetClaimPath = table.Column<string>(type: "TEXT", nullable: true),
                    IncludeInAccessToken = table.Column<bool>(type: "INTEGER", nullable: false),
                    SAMLAttributeName = table.Column<string>(type: "TEXT", nullable: true),
                    TokenClaimJsonType = table.Column<int>(type: "INTEGER", nullable: true),
                    IsMultiValued = table.Column<bool>(type: "INTEGER", nullable: false),
                    ScopeId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeClaimMapper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScopeClaimMapper_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealmSerializedFileKey",
                columns: table => new
                {
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false),
                    SerializedFileKeysId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealmSerializedFileKey", x => new { x.RealmsName, x.SerializedFileKeysId });
                    table.ForeignKey(
                        name: "FK_RealmSerializedFileKey_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealmSerializedFileKey_SerializedFileKeys_SerializedFileKeysId",
                        column: x => x.SerializedFileKeysId,
                        principalTable: "SerializedFileKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UMAPermissionTicketRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResourceId = table.Column<string>(type: "TEXT", nullable: false),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false),
                    UMAPermissionTicketId = table.Column<string>(type: "TEXT", nullable: true)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TicketId = table.Column<string>(type: "TEXT", nullable: false),
                    Requester = table.Column<string>(type: "TEXT", nullable: true),
                    Owner = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourceId = table.Column<string>(type: "TEXT", nullable: false),
                    Realm = table.Column<string>(type: "TEXT", nullable: false),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false)
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
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false),
                    UMAResourceId = table.Column<string>(type: "TEXT", nullable: true)
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
                name: "AuthenticationSchemeProviderMapper",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SourceClaimName = table.Column<string>(type: "TEXT", nullable: true),
                    MapperType = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetUserAttribute = table.Column<string>(type: "TEXT", nullable: true),
                    TargetUserProperty = table.Column<string>(type: "TEXT", nullable: true),
                    IdProviderId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderMapper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderMapper_AuthenticationSchemeProviders_IdProviderId",
                        column: x => x.IdProviderId,
                        principalTable: "AuthenticationSchemeProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderRealm",
                columns: table => new
                {
                    AuthenticationSchemeProvidersId = table.Column<string>(type: "TEXT", nullable: false),
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderRealm", x => new { x.AuthenticationSchemeProvidersId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderRealm_AuthenticationSchemeProviders_AuthenticationSchemeProvidersId",
                        column: x => x.AuthenticationSchemeProvidersId,
                        principalTable: "AuthenticationSchemeProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderRealm_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationUMAResource",
                columns: table => new
                {
                    TranslationsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UMAResourceId = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "ConfigurationDefinitionRecordTranslation",
                columns: table => new
                {
                    ConfigurationDefinitionRecordId = table.Column<string>(type: "TEXT", nullable: false),
                    TranslationsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordTranslation", x => new { x.ConfigurationDefinitionRecordId, x.TranslationsId });
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordTranslation_ConfigurationDefinitionRecord_ConfigurationDefinitionRecordId",
                        column: x => x.ConfigurationDefinitionRecordId,
                        principalTable: "ConfigurationDefinitionRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordTranslation_Translations_TranslationsId",
                        column: x => x.TranslationsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordValue",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    ConfigurationDefinitionRecordId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecord_ConfigurationDefinitionRecordId",
                        column: x => x.ConfigurationDefinitionRecordId,
                        principalTable: "ConfigurationDefinitionRecord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FolderName = table.Column<string>(type: "TEXT", nullable: true),
                    NbRepresentations = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    IdentityProvisioningId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningHistory_IdentityProvisioningLst_IdentityProvisioningId",
                        column: x => x.IdentityProvisioningId,
                        principalTable: "IdentityProvisioningLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningRealm",
                columns: table => new
                {
                    IdentityProvisioningLstId = table.Column<string>(type: "TEXT", nullable: false),
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningRealm", x => new { x.IdentityProvisioningLstId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningRealm_IdentityProvisioningLst_IdentityProvisioningLstId",
                        column: x => x.IdentityProvisioningLstId,
                        principalTable: "IdentityProvisioningLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningRealm_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Firstname = table.Column<string>(type: "TEXT", nullable: true),
                    Lastname = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    EmailVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeviceRegistrationToken = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    IdentityProvisioningId = table.Column<string>(type: "TEXT", nullable: true),
                    Did = table.Column<string>(type: "TEXT", nullable: true),
                    DidPrivateHex = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationMode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_IdentityProvisioningLst_IdentityProvisioningId",
                        column: x => x.IdentityProvisioningId,
                        principalTable: "IdentityProvisioningLst",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UMAResourcePermissionClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    FriendlyName = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    UMAResourcePermissionId = table.Column<string>(type: "TEXT", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordValueTranslation",
                columns: table => new
                {
                    ConfigurationDefinitionRecordValueId = table.Column<string>(type: "TEXT", nullable: false),
                    TranslationsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationDefinitionRecordValueTranslation", x => new { x.ConfigurationDefinitionRecordValueId, x.TranslationsId });
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValueTranslation_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordValueId",
                        column: x => x.ConfigurationDefinitionRecordValueId,
                        principalTable: "ConfigurationDefinitionRecordValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationDefinitionRecordValueTranslation_Translations_TranslationsId",
                        column: x => x.TranslationsId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialOffers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CredentialTemplateId = table.Column<string>(type: "TEXT", nullable: false),
                    CredentialNames = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Pin = table.Column<string>(type: "TEXT", nullable: true),
                    PreAuthorizedCode = table.Column<string>(type: "TEXT", nullable: true),
                    CredIssuerState = table.Column<string>(type: "TEXT", nullable: true)
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
                name: "DeviceAuthCodes",
                columns: table => new
                {
                    DeviceCode = table.Column<string>(type: "TEXT", nullable: false),
                    UserCode = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    UserLogin = table.Column<string>(type: "TEXT", nullable: true),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextAccessDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LastAccessTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceAuthCodes", x => x.DeviceCode);
                    table.ForeignKey(
                        name: "FK_DeviceAuthCodes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceAuthCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Grants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Claims = table.Column<string>(type: "TEXT", nullable: false),
                    SerializedAuthorizationDetails = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Realm = table.Column<string>(type: "TEXT", nullable: false),
                    ScopeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grants_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Grants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUser",
                columns: table => new
                {
                    GroupsId = table.Column<string>(type: "TEXT", nullable: false),
                    UsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUser", x => new { x.GroupsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_GroupUser_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealmUser",
                columns: table => new
                {
                    RealmsName = table.Column<string>(type: "TEXT", nullable: false),
                    UsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealmUser", x => new { x.UsersId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_RealmUser_Realms_RealmsName",
                        column: x => x.RealmsName,
                        principalTable: "Realms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealmUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredential",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CredentialType = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    OTPAlg = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    OTPCounter = table.Column<int>(type: "INTEGER", nullable: false),
                    TOTPStep = table.Column<int>(type: "INTEGER", nullable: false),
                    HOTPWindow = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
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
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceType = table.Column<string>(type: "TEXT", nullable: true),
                    Model = table.Column<string>(type: "TEXT", nullable: true),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: true),
                    PushToken = table.Column<string>(type: "TEXT", nullable: true),
                    PushType = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Scheme = table.Column<string>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
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
                    SessionId = table.Column<string>(type: "TEXT", nullable: false),
                    AuthenticationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    Realm = table.Column<string>(type: "TEXT", nullable: false),
                    IsClientsNotified = table.Column<bool>(type: "INTEGER", nullable: false),
                    SerializedClientIds = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "AuthorizedScope",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Scope = table.Column<string>(type: "TEXT", nullable: false),
                    ConsentId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedScope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizedScope_Grants_ConsentId",
                        column: x => x.ConsentId,
                        principalTable: "Grants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizedResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Resource = table.Column<string>(type: "TEXT", nullable: false),
                    Audience = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorizedScopeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizedResource_AuthorizedScope_AuthorizedScopeId",
                        column: x => x.AuthorizedScopeId,
                        principalTable: "AuthorizedScope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceRealm_RealmsName",
                table: "ApiResourceRealm",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResourceScope_ScopesId",
                table: "ApiResourceScope",
                column: "ScopesId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationContextClassReferenceRealm_RealmsName",
                table: "AuthenticationContextClassReferenceRealm",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderMapper_IdProviderId",
                table: "AuthenticationSchemeProviderMapper",
                column: "IdProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderRealm_RealmsName",
                table: "AuthenticationSchemeProviderRealm",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviders_AuthSchemeProviderDefinitionName",
                table: "AuthenticationSchemeProviders",
                column: "AuthSchemeProviderDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedResource_AuthorizedScopeId",
                table: "AuthorizedResource",
                column: "AuthorizedScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedScope_ConsentId",
                table: "AuthorizedScope",
                column: "ConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_BCAuthorizeHistory_BCAuthorizeId",
                table: "BCAuthorizeHistory",
                column: "BCAuthorizeId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateAuthorityRealm_RealmsName",
                table: "CertificateAuthorityRealm",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCertificate_CertificateAuthorityId",
                table: "ClientCertificate",
                column: "CertificateAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientJsonWebKey_ClientId",
                table: "ClientJsonWebKey",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRealm_RealmsName",
                table: "ClientRealm",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_ClientScope_ScopesId",
                table: "ClientScope",
                column: "ScopesId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDefinitionRecord_ConfigurationDefinitionId",
                table: "ConfigurationDefinitionRecord",
                column: "ConfigurationDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDefinitionRecordTranslation_TranslationsId",
                table: "ConfigurationDefinitionRecordTranslation",
                column: "TranslationsId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDefinitionRecordValue_ConfigurationDefinitionRecordId",
                table: "ConfigurationDefinitionRecordValue",
                column: "ConfigurationDefinitionRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationDefinitionRecordValueTranslation_TranslationsId",
                table: "ConfigurationDefinitionRecordValueTranslation",
                column: "TranslationsId");

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

            migrationBuilder.CreateIndex(
                name: "IX_DeviceAuthCodes_ClientId",
                table: "DeviceAuthCodes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceAuthCodes_UserId",
                table: "DeviceAuthCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Grants_ScopeId",
                table: "Grants",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Grants_UserId",
                table: "Grants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRealm_RealmsName",
                table: "GroupRealm",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ParentGroupId",
                table: "Groups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupScope_RolesId",
                table: "GroupScope",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_UsersId",
                table: "GroupUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningHistory_IdentityProvisioningId",
                table: "IdentityProvisioningHistory",
                column: "IdentityProvisioningId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningLst_DefinitionName",
                table: "IdentityProvisioningLst",
                column: "DefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningMappingRule_IdentityProvisioningDefinitionName",
                table: "IdentityProvisioningMappingRule",
                column: "IdentityProvisioningDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningRealm_RealmsName",
                table: "IdentityProvisioningRealm",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_ImportSummaries_RealmName",
                table: "ImportSummaries",
                column: "RealmName");

            migrationBuilder.CreateIndex(
                name: "IX_RealmScope_ScopesId",
                table: "RealmScope",
                column: "ScopesId");

            migrationBuilder.CreateIndex(
                name: "IX_RealmSerializedFileKey_SerializedFileKeysId",
                table: "RealmSerializedFileKey",
                column: "SerializedFileKeysId");

            migrationBuilder.CreateIndex(
                name: "IX_RealmUser_RealmsName",
                table: "RealmUser",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationWorkflows_RealmName",
                table: "RegistrationWorkflows",
                column: "RealmName");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeClaimMapper_ScopeId",
                table: "ScopeClaimMapper",
                column: "ScopeId");

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
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
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
                name: "IX_Users_IdentityProvisioningId",
                table: "Users",
                column: "IdentityProvisioningId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserId",
                table: "UserSession",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiResourceRealm");

            migrationBuilder.DropTable(
                name: "ApiResourceScope");

            migrationBuilder.DropTable(
                name: "AuditEvents");

            migrationBuilder.DropTable(
                name: "AuthenticationContextClassReferenceRealm");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderMapper");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderRealm");

            migrationBuilder.DropTable(
                name: "AuthorizedResource");

            migrationBuilder.DropTable(
                name: "BCAuthorizeHistory");

            migrationBuilder.DropTable(
                name: "CertificateAuthorityRealm");

            migrationBuilder.DropTable(
                name: "ClaimProviders");

            migrationBuilder.DropTable(
                name: "ClientCertificate");

            migrationBuilder.DropTable(
                name: "ClientJsonWebKey");

            migrationBuilder.DropTable(
                name: "ClientRealm");

            migrationBuilder.DropTable(
                name: "ClientScope");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecordTranslation");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecordValueTranslation");

            migrationBuilder.DropTable(
                name: "ConfigurationKeyPairValueRecords");

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
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "DeviceAuthCodes");

            migrationBuilder.DropTable(
                name: "ExtractedRepresentations");

            migrationBuilder.DropTable(
                name: "GroupRealm");

            migrationBuilder.DropTable(
                name: "GroupScope");

            migrationBuilder.DropTable(
                name: "GroupUser");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningHistory");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningMappingRule");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningRealm");

            migrationBuilder.DropTable(
                name: "ImportSummaries");

            migrationBuilder.DropTable(
                name: "Networks");

            migrationBuilder.DropTable(
                name: "RealmScope");

            migrationBuilder.DropTable(
                name: "RealmSerializedFileKey");

            migrationBuilder.DropTable(
                name: "RealmUser");

            migrationBuilder.DropTable(
                name: "RegistrationWorkflows");

            migrationBuilder.DropTable(
                name: "ScopeClaimMapper");

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
                name: "UserClaims");

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
                name: "Acrs");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviders");

            migrationBuilder.DropTable(
                name: "AuthorizedScope");

            migrationBuilder.DropTable(
                name: "BCAuthorizeLst");

            migrationBuilder.DropTable(
                name: "CertificateAuthorities");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecordValue");

            migrationBuilder.DropTable(
                name: "BaseCredentialTemplate");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "SerializedFileKeys");

            migrationBuilder.DropTable(
                name: "Realms");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "UMAPermissionTicket");

            migrationBuilder.DropTable(
                name: "UMAResourcePermission");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderDefinitions");

            migrationBuilder.DropTable(
                name: "Grants");

            migrationBuilder.DropTable(
                name: "ConfigurationDefinitionRecord");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "UmaResources");

            migrationBuilder.DropTable(
                name: "Scopes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Definitions");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningLst");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningDefinitions");
        }
    }
}
