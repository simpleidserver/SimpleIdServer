using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SimpleIdServer.IdServer.PostgreMigrations.Migrations
{
    /// <inheritdoc />
    public partial class InitPostgre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Acrs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    AuthenticationMethodReferences = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acrs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiResources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: false),
                    IsError = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    RequestJSON = table.Column<string>(type: "text", nullable: true),
                    RedirectUrl = table.Column<string>(type: "text", nullable: true),
                    AuthMethod = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: false),
                    Claims = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderDefinitions",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    HandlerFullQualifiedName = table.Column<string>(type: "text", nullable: true),
                    OptionsFullQualifiedName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderDefinitions", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "BaseCredentialTemplate",
                columns: table => new
                {
                    TechnicalId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true),
                    Format = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseCredentialTemplate", x => x.TechnicalId);
                });

            migrationBuilder.CreateTable(
                name: "BCAuthorizeLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    NotificationToken = table.Column<string>(type: "text", nullable: true),
                    NotificationMode = table.Column<string>(type: "text", nullable: true),
                    NotificationEdp = table.Column<string>(type: "text", nullable: true),
                    Interval = table.Column<int>(type: "integer", nullable: true),
                    LastStatus = table.Column<int>(type: "integer", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RejectionSentDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextFetchTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Realm = table.Column<string>(type: "text", nullable: false),
                    SerializedAuthorizationDetails = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCAuthorizeLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateAuthorities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    SubjectName = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    StoreLocation = table.Column<int>(type: "integer", nullable: true),
                    StoreName = table.Column<int>(type: "integer", nullable: true),
                    FindType = table.Column<int>(type: "integer", nullable: true),
                    FindValue = table.Column<string>(type: "text", nullable: true),
                    PublicKey = table.Column<string>(type: "text", nullable: true),
                    PrivateKey = table.Column<string>(type: "text", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateAuthorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClaimProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProviderType = table.Column<string>(type: "text", nullable: false),
                    ConnectionString = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ClientSecret = table.Column<string>(type: "text", nullable: false),
                    RegistrationAccessToken = table.Column<string>(type: "text", nullable: true),
                    GrantTypes = table.Column<string>(type: "text", nullable: false),
                    RedirectionUrls = table.Column<string>(type: "text", nullable: false),
                    TokenEndPointAuthMethod = table.Column<string>(type: "text", nullable: true),
                    ResponseTypes = table.Column<string>(type: "text", nullable: false),
                    JwksUri = table.Column<string>(type: "text", nullable: true),
                    Contacts = table.Column<string>(type: "text", nullable: false),
                    SoftwareId = table.Column<string>(type: "text", nullable: true),
                    SoftwareVersion = table.Column<string>(type: "text", nullable: true),
                    TlsClientAuthSubjectDN = table.Column<string>(type: "text", nullable: true),
                    TlsClientAuthSanDNS = table.Column<string>(type: "text", nullable: true),
                    TlsClientAuthSanURI = table.Column<string>(type: "text", nullable: true),
                    TlsClientAuthSanIP = table.Column<string>(type: "text", nullable: true),
                    TlsClientAuthSanEmail = table.Column<string>(type: "text", nullable: true),
                    ClientSecretExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TokenExpirationTimeInSeconds = table.Column<double>(type: "double precision", nullable: true),
                    CNonceExpirationTimeInSeconds = table.Column<double>(type: "double precision", nullable: true),
                    RefreshTokenExpirationTimeInSeconds = table.Column<double>(type: "double precision", nullable: true),
                    TokenSignedResponseAlg = table.Column<string>(type: "text", nullable: true),
                    TokenEncryptedResponseAlg = table.Column<string>(type: "text", nullable: true),
                    TokenEncryptedResponseEnc = table.Column<string>(type: "text", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "text", nullable: false),
                    PreferredTokenProfile = table.Column<string>(type: "text", nullable: true),
                    RequestObjectSigningAlg = table.Column<string>(type: "text", nullable: true),
                    RequestObjectEncryptionAlg = table.Column<string>(type: "text", nullable: true),
                    RequestObjectEncryptionEnc = table.Column<string>(type: "text", nullable: true),
                    SubjectType = table.Column<string>(type: "text", nullable: true),
                    PairWiseIdentifierSalt = table.Column<string>(type: "text", nullable: true),
                    SectorIdentifierUri = table.Column<string>(type: "text", nullable: true),
                    IdTokenSignedResponseAlg = table.Column<string>(type: "text", nullable: true),
                    IdTokenEncryptedResponseAlg = table.Column<string>(type: "text", nullable: true),
                    IdTokenEncryptedResponseEnc = table.Column<string>(type: "text", nullable: true),
                    BCTokenDeliveryMode = table.Column<string>(type: "text", nullable: true),
                    BCClientNotificationEndpoint = table.Column<string>(type: "text", nullable: true),
                    BCAuthenticationRequestSigningAlg = table.Column<string>(type: "text", nullable: true),
                    UserInfoSignedResponseAlg = table.Column<string>(type: "text", nullable: true),
                    UserInfoEncryptedResponseAlg = table.Column<string>(type: "text", nullable: true),
                    UserInfoEncryptedResponseEnc = table.Column<string>(type: "text", nullable: true),
                    BCUserCodeParameter = table.Column<bool>(type: "boolean", nullable: false),
                    FrontChannelLogoutUri = table.Column<string>(type: "text", nullable: true),
                    CredentialOfferEndpoint = table.Column<string>(type: "text", nullable: true),
                    UserPinRequired = table.Column<bool>(type: "boolean", nullable: false),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(type: "boolean", nullable: false),
                    BackChannelLogoutSessionRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultMaxAge = table.Column<double>(type: "double precision", nullable: true),
                    TlsClientCertificateBoundAccessToken = table.Column<bool>(type: "boolean", nullable: false),
                    BackChannelLogoutUri = table.Column<string>(type: "text", nullable: true),
                    ApplicationType = table.Column<string>(type: "text", nullable: true),
                    InitiateLoginUri = table.Column<string>(type: "text", nullable: true),
                    RequireAuthTime = table.Column<bool>(type: "boolean", nullable: false),
                    AuthorizationSignedResponseAlg = table.Column<string>(type: "text", nullable: true),
                    AuthorizationEncryptedResponseAlg = table.Column<string>(type: "text", nullable: true),
                    AuthorizationDataTypes = table.Column<string>(type: "text", nullable: false),
                    AuthorizationEncryptedResponseEnc = table.Column<string>(type: "text", nullable: true),
                    IsConsentDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsResourceParameterRequired = table.Column<bool>(type: "boolean", nullable: false),
                    AuthReqIdExpirationTimeInSeconds = table.Column<int>(type: "integer", nullable: false),
                    ClientType = table.Column<string>(type: "text", nullable: true),
                    SerializedParameters = table.Column<string>(type: "text", nullable: true),
                    DefaultAcrValues = table.Column<string>(type: "text", nullable: false),
                    BCIntervalSeconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtractedRepresentations",
                columns: table => new
                {
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractedRepresentations", x => x.ExternalId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FullPath = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ParentGroupId = table.Column<string>(type: "text", nullable: true)
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
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningDefinitions", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Networks",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    RpcUrl = table.Column<string>(type: "text", nullable: false),
                    PrivateAccountKey = table.Column<string>(type: "text", nullable: false),
                    ContractAdr = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Networks", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Protocol = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsExposedInConfigurationEdp = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerializedFileKeys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    KeyId = table.Column<string>(type: "text", nullable: false),
                    Usage = table.Column<string>(type: "text", nullable: false),
                    Alg = table.Column<string>(type: "text", nullable: false),
                    Enc = table.Column<string>(type: "text", nullable: true),
                    PublicKeyPem = table.Column<string>(type: "text", nullable: true),
                    PrivateKeyPem = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSymmetric = table.Column<bool>(type: "boolean", nullable: false),
                    Key = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerializedFileKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    PkID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    TokenType = table.Column<string>(type: "text", nullable: false),
                    IsRegistrationAccessToken = table.Column<bool>(type: "boolean", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: true),
                    OriginalData = table.Column<string>(type: "text", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "text", nullable: true),
                    GrantId = table.Column<string>(type: "text", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.PkID);
                });

            migrationBuilder.CreateTable(
                name: "UMAPermissionTicket",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAPermissionTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UmaResources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IconUri = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Scopes = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmaResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderDefinitionProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SchemeProviderDefName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderDefinitionProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderDefinitionProperty_Authenticati~",
                        column: x => x.SchemeProviderDefName,
                        principalTable: "AuthenticationSchemeProviderDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthSchemeProviderDefinitionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviders_AuthenticationSchemeProviderD~",
                        column: x => x.AuthSchemeProviderDefinitionName,
                        principalTable: "AuthenticationSchemeProviderDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialTemplateClaimMapper",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MapperType = table.Column<int>(type: "integer", nullable: false),
                    SourceUserAttribute = table.Column<string>(type: "text", nullable: true),
                    SourceUserProperty = table.Column<string>(type: "text", nullable: true),
                    TargetClaimPath = table.Column<string>(type: "text", nullable: false),
                    IsMultiValued = table.Column<bool>(type: "boolean", nullable: false),
                    TokenClaimJsonType = table.Column<int>(type: "integer", nullable: true),
                    CredentialTemplateId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialTemplateClaimMapper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialTemplateClaimMapper_BaseCredentialTemplate_Creden~",
                        column: x => x.CredentialTemplateId,
                        principalTable: "BaseCredentialTemplate",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialTemplateDisplay",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Locale = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    LogoAltText = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BackgroundColor = table.Column<string>(type: "text", nullable: true),
                    TextColor = table.Column<string>(type: "text", nullable: true),
                    CredentialTemplateId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialTemplateDisplay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialTemplateDisplay_BaseCredentialTemplate_Credential~",
                        column: x => x.CredentialTemplateId,
                        principalTable: "BaseCredentialTemplate",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialTemplateParameter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CredentialTemplateId = table.Column<string>(type: "text", nullable: true),
                    ParameterType = table.Column<int>(type: "integer", nullable: false),
                    IsArray = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialTemplateParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CredentialTemplateParameter_BaseCredentialTemplate_Credenti~",
                        column: x => x.CredentialTemplateId,
                        principalTable: "BaseCredentialTemplate",
                        principalColumn: "TechnicalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BCAuthorizeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BCAuthorizeId = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublicKey = table.Column<string>(type: "text", nullable: false),
                    PrivateKey = table.Column<string>(type: "text", nullable: false),
                    CertificateAuthorityId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCertificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientCertificate_CertificateAuthorities_CertificateAuthori~",
                        column: x => x.CertificateAuthorityId,
                        principalTable: "CertificateAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientJsonWebKey",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kid = table.Column<string>(type: "text", nullable: false),
                    Alg = table.Column<string>(type: "text", nullable: false),
                    Usage = table.Column<string>(type: "text", nullable: false),
                    KeyType = table.Column<int>(type: "integer", nullable: true),
                    SerializedJsonWebKey = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: true)
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
                name: "IdentityProvisioningDefinitionProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IdentityProvisioningDefinitionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningDefinitionProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningDefinitionProperty_IdentityProvisioning~",
                        column: x => x.IdentityProvisioningDefinitionName,
                        principalTable: "IdentityProvisioningDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DefinitionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningLst", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningLst_IdentityProvisioningDefinitions_Def~",
                        column: x => x.DefinitionName,
                        principalTable: "IdentityProvisioningDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningMappingRule",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    From = table.Column<string>(type: "text", nullable: false),
                    MapperType = table.Column<int>(type: "integer", nullable: false),
                    TargetUserAttribute = table.Column<string>(type: "text", nullable: true),
                    TargetUserProperty = table.Column<string>(type: "text", nullable: true),
                    IdentityProvisioningDefinitionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningMappingRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningMappingRule_IdentityProvisioningDefinit~",
                        column: x => x.IdentityProvisioningDefinitionName,
                        principalTable: "IdentityProvisioningDefinitions",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiResourceRealm",
                columns: table => new
                {
                    ApiResourcesId = table.Column<string>(type: "text", nullable: false),
                    RealmsName = table.Column<string>(type: "text", nullable: false)
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
                    AuthenticationContextClassReferencesId = table.Column<string>(type: "text", nullable: false),
                    RealmsName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationContextClassReferenceRealm", x => new { x.AuthenticationContextClassReferencesId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_AuthenticationContextClassReferenceRealm_Acrs_Authenticatio~",
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
                    CertificateAuthoritiesId = table.Column<string>(type: "text", nullable: false),
                    RealmsName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateAuthorityRealm", x => new { x.CertificateAuthoritiesId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_CertificateAuthorityRealm_CertificateAuthorities_Certificat~",
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
                    ClientsId = table.Column<string>(type: "text", nullable: false),
                    RealmsName = table.Column<string>(type: "text", nullable: false)
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
                    CredentialTemplatesTechnicalId = table.Column<string>(type: "text", nullable: false),
                    RealmsName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialTemplateRealm", x => new { x.CredentialTemplatesTechnicalId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_CredentialTemplateRealm_BaseCredentialTemplate_CredentialTe~",
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
                    GroupsId = table.Column<string>(type: "text", nullable: false),
                    RealmsName = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NbRepresentations = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RealmName = table.Column<string>(type: "text", nullable: false)
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
                name: "ApiResourceScope",
                columns: table => new
                {
                    ApiResourcesId = table.Column<string>(type: "text", nullable: false),
                    ScopesId = table.Column<string>(type: "text", nullable: false)
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
                    ClientsId = table.Column<string>(type: "text", nullable: false),
                    ScopesId = table.Column<string>(type: "text", nullable: false)
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
                    GroupsId = table.Column<string>(type: "text", nullable: false),
                    RolesId = table.Column<string>(type: "text", nullable: false)
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
                    RealmsName = table.Column<string>(type: "text", nullable: false),
                    ScopesId = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MapperType = table.Column<int>(type: "integer", nullable: false),
                    SourceUserAttribute = table.Column<string>(type: "text", nullable: true),
                    SourceUserProperty = table.Column<string>(type: "text", nullable: true),
                    TargetClaimPath = table.Column<string>(type: "text", nullable: true),
                    SAMLAttributeName = table.Column<string>(type: "text", nullable: true),
                    TokenClaimJsonType = table.Column<int>(type: "integer", nullable: true),
                    IsMultiValued = table.Column<bool>(type: "boolean", nullable: false),
                    ScopeId = table.Column<string>(type: "text", nullable: false)
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
                    RealmsName = table.Column<string>(type: "text", nullable: false),
                    SerializedFileKeysId = table.Column<string>(type: "text", nullable: false)
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
                        name: "FK_RealmSerializedFileKey_SerializedFileKeys_SerializedFileKey~",
                        column: x => x.SerializedFileKeysId,
                        principalTable: "SerializedFileKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UMAPermissionTicketRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResourceId = table.Column<string>(type: "text", nullable: false),
                    Scopes = table.Column<string>(type: "text", nullable: false),
                    UMAPermissionTicketId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAPermissionTicketRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UMAPermissionTicketRecord_UMAPermissionTicket_UMAPermission~",
                        column: x => x.UMAPermissionTicketId,
                        principalTable: "UMAPermissionTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UmaPendingRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<string>(type: "text", nullable: false),
                    Requester = table.Column<string>(type: "text", nullable: true),
                    Owner = table.Column<string>(type: "text", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: false),
                    Scopes = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Scopes = table.Column<string>(type: "text", nullable: false),
                    UMAResourceId = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SourceClaimName = table.Column<string>(type: "text", nullable: true),
                    MapperType = table.Column<int>(type: "integer", nullable: false),
                    TargetUserAttribute = table.Column<string>(type: "text", nullable: true),
                    TargetUserProperty = table.Column<string>(type: "text", nullable: true),
                    IdProviderId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderMapper", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderMapper_AuthenticationSchemeProv~",
                        column: x => x.IdProviderId,
                        principalTable: "AuthenticationSchemeProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    SchemeProviderId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderProperty_AuthenticationSchemePr~",
                        column: x => x.SchemeProviderId,
                        principalTable: "AuthenticationSchemeProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderRealm",
                columns: table => new
                {
                    AuthenticationSchemeProvidersId = table.Column<string>(type: "text", nullable: false),
                    RealmsName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderRealm", x => new { x.AuthenticationSchemeProvidersId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_AuthenticationSchemeProviderRealm_AuthenticationSchemeProvi~",
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
                    TranslationsId = table.Column<int>(type: "integer", nullable: false),
                    UMAResourceId = table.Column<string>(type: "text", nullable: false)
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
                name: "IdentityProvisioningHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FolderName = table.Column<string>(type: "text", nullable: true),
                    NbRepresentations = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    IdentityProvisioningId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningHistory_IdentityProvisioningLst_Identit~",
                        column: x => x.IdentityProvisioningId,
                        principalTable: "IdentityProvisioningLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    IdentityProvisioningId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningProperty_IdentityProvisioningLst_Identi~",
                        column: x => x.IdentityProvisioningId,
                        principalTable: "IdentityProvisioningLst",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProvisioningRealm",
                columns: table => new
                {
                    IdentityProvisioningLstId = table.Column<string>(type: "text", nullable: false),
                    RealmsName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningRealm", x => new { x.IdentityProvisioningLstId, x.RealmsName });
                    table.ForeignKey(
                        name: "FK_IdentityProvisioningRealm_IdentityProvisioningLst_IdentityP~",
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Firstname = table.Column<string>(type: "text", nullable: true),
                    Lastname = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    DeviceRegistrationToken = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    IdentityProvisioningId = table.Column<string>(type: "text", nullable: true),
                    Did = table.Column<string>(type: "text", nullable: true),
                    DidPrivateHex = table.Column<string>(type: "text", nullable: true),
                    NotificationMode = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    UMAResourcePermissionId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAResourcePermissionClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UMAResourcePermissionClaim_UMAResourcePermission_UMAResourc~",
                        column: x => x.UMAResourcePermissionId,
                        principalTable: "UMAResourcePermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CredentialOffers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CredentialTemplateId = table.Column<string>(type: "text", nullable: false),
                    CredentialNames = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Pin = table.Column<string>(type: "text", nullable: true),
                    PreAuthorizedCode = table.Column<string>(type: "text", nullable: true),
                    CredIssuerState = table.Column<string>(type: "text", nullable: true)
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
                    DeviceCode = table.Column<string>(type: "text", nullable: false),
                    UserCode = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    UserLogin = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextAccessDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LastAccessTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: false),
                    Claims = table.Column<string>(type: "text", nullable: false),
                    SerializedAuthorizationDetails = table.Column<string>(type: "text", nullable: true),
                    ScopeId = table.Column<string>(type: "text", nullable: true)
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
                    GroupsId = table.Column<string>(type: "text", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
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
                    RealmsName = table.Column<string>(type: "text", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    CredentialType = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    OTPAlg = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    OTPCounter = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    SerializedOptions = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Scheme = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    AuthenticationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    Resources = table.Column<string>(type: "text", nullable: false),
                    ConsentId = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_AuthenticationSchemeProviderDefinitionProperty_SchemeProvid~",
                table: "AuthenticationSchemeProviderDefinitionProperty",
                column: "SchemeProviderDefName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderMapper_IdProviderId",
                table: "AuthenticationSchemeProviderMapper",
                column: "IdProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderProperty_SchemeProviderId",
                table: "AuthenticationSchemeProviderProperty",
                column: "SchemeProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviderRealm_RealmsName",
                table: "AuthenticationSchemeProviderRealm",
                column: "RealmsName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationSchemeProviders_AuthSchemeProviderDefinitionN~",
                table: "AuthenticationSchemeProviders",
                column: "AuthSchemeProviderDefinitionName");

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
                name: "IX_IdentityProvisioningDefinitionProperty_IdentityProvisioning~",
                table: "IdentityProvisioningDefinitionProperty",
                column: "IdentityProvisioningDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningHistory_IdentityProvisioningId",
                table: "IdentityProvisioningHistory",
                column: "IdentityProvisioningId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningLst_DefinitionName",
                table: "IdentityProvisioningLst",
                column: "DefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningMappingRule_IdentityProvisioningDefinit~",
                table: "IdentityProvisioningMappingRule",
                column: "IdentityProvisioningDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProvisioningProperty_IdentityProvisioningId",
                table: "IdentityProvisioningProperty",
                column: "IdentityProvisioningId");

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
                name: "AuthenticationSchemeProviderDefinitionProperty");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderMapper");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderProperty");

            migrationBuilder.DropTable(
                name: "AuthenticationSchemeProviderRealm");

            migrationBuilder.DropTable(
                name: "AuthorizedScope");

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
                name: "IdentityProvisioningDefinitionProperty");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningHistory");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningMappingRule");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningProperty");

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
                name: "Grants");

            migrationBuilder.DropTable(
                name: "BCAuthorizeLst");

            migrationBuilder.DropTable(
                name: "CertificateAuthorities");

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
                name: "Scopes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "UmaResources");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningLst");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningDefinitions");
        }
    }
}
