using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleIdServer.IdServer.OracleMigrations.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiResources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Audience = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    EventName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Realm = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsError = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ClientId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequestJSON = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RedirectUrl = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AuthMethod = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Scopes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Claims = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviderDefinitions",
                columns: table => new
                {
                    Name = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Image = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    HandlerFullQualifiedName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    OptionsFullQualifiedName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    OptionsName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationSchemeProviderDefinitions", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "BCAuthorizeLst",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClientId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NotificationToken = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NotificationMode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NotificationEdp = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Interval = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    LastStatus = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RejectionSentDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NextFetchTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Realm = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SerializedAuthorizationDetails = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Scopes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCAuthorizeLst", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateAuthorities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    SubjectName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Source = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StoreLocation = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    StoreName = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    FindType = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    FindValue = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PublicKey = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PrivateKey = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateAuthorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClaimProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ProviderType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ConnectionString = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ClaimType = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationKeyPairValueRecords",
                columns: table => new
                {
                    Name = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationKeyPairValueRecords", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FriendlyName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Xml = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Definitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FullQualifiedName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtractedRepresentations",
                columns: table => new
                {
                    ExternalId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Version = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractedRepresentations", x => x.ExternalId);
                });

            migrationBuilder.CreateTable(
                name: "ExtractedRepresentationsStaging",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RepresentationId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RepresentationVersion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Values = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IdProvisioningProcessId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    GroupIds = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Type = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractedRepresentationsStaging", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GotifySessions",
                columns: table => new
                {
                    ApplicationToken = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClientToken = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GotifySessions", x => x.ApplicationToken);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    FullPath = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ParentGroupId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                    Name = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OptionsName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    OptionsFullQualifiedName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProvisioningDefinitions", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Code = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "MessageBusErrorMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ExternalId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    FullName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Content = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Exceptions = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    QueueName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBusErrorMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    Name = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Protocol = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsExposedInConfigurationEdp = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerializedFileKeys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    KeyId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Usage = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Alg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Enc = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PublicKeyPem = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PrivateKeyPem = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IsSymmetric = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Key = table.Column<byte[]>(type: "RAW(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerializedFileKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    PkID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Id = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SessionId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClientId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TokenType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AccessTokenType = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Data = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    OriginalData = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    GrantId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Jkt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.PkID);
                });

            migrationBuilder.CreateTable(
                name: "UMAPermissionTicket",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UMAPermissionTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UmaResources",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    IconUri = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Type = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Subject = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Scopes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Realm = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmaResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationSchemeProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DisplayName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AuthSchemeProviderDefinitionName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "BCAuthorizeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    StartDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    BCAuthorizeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PublicKey = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PrivateKey = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CertificateAuthorityId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "ConfigurationDefinitionRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsRequired = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DisplayCondition = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ConfigurationDefinitionId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DefinitionName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    From = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MapperType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TargetUserAttribute = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TargetUserProperty = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    HasMultipleAttribute = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Usage = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IdentityProvisioningDefinitionName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    ApiResourcesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "CertificateAuthorityRealm",
                columns: table => new
                {
                    CertificateAuthoritiesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "GroupRealm",
                columns: table => new
                {
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    GroupsId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "PresentationDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    PublicId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Purpose = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RealmName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "RegistrationWorkflows",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RealmName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Steps = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsDefault = table.Column<bool>(type: "BOOLEAN", nullable: false)
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
                    ApiResourcesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ScopesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "GroupScope",
                columns: table => new
                {
                    GroupsId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RolesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ScopesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MapperType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SourceUserAttribute = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SourceUserProperty = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TargetClaimPath = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IncludeInAccessToken = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    SAMLAttributeName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TokenClaimJsonType = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    IsMultiValued = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    ScopeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    SerializedFileKeysId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ResourceId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Scopes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    UMAPermissionTicketId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TicketId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Requester = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Owner = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ResourceId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Realm = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Scopes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Scopes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    UMAResourceId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SourceClaimName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MapperType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TargetUserAttribute = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TargetUserProperty = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IdProviderId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    AuthenticationSchemeProvidersId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "ConfigurationDefinitionRecordValue",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ConfigurationDefinitionRecordId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ProcessId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ExecutionDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CurrentPage = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NbUsers = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NbGroups = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NbFilteredRepresentations = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TotalPages = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IdentityProvisioningId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    IdentityProvisioningLstId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Firstname = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Lastname = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EmailVerified = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    DeviceRegistrationToken = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Source = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UnblockDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NbLoginAttempt = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IdentityProvisioningId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    EncodedPicture = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NotificationMode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
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
                name: "PresentationDefinitionInputDescriptor",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    PublicId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Purpose = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PresentationDefinitionId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionInputDescriptor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionInputDescriptor_PresentationDefinitions_PresentationDefinitionId",
                        column: x => x.PresentationDefinitionId,
                        principalTable: "PresentationDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Acrs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DisplayName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AuthenticationMethodReferences = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    RegistrationWorkflowId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acrs_RegistrationWorkflows_RegistrationWorkflowId",
                        column: x => x.RegistrationWorkflowId,
                        principalTable: "RegistrationWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UMAResourcePermissionClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ClaimType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FriendlyName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    UMAResourcePermissionId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                name: "Grants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClientId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Claims = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SerializedAuthorizationDetails = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Realm = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ScopeId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                    GroupsId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UsersId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UsersId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Type = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CredentialType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    OTPAlg = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    OTPCounter = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TOTPStep = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    HOTPWindow = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    DeviceType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Model = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Manufacturer = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Version = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PushToken = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PushType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Scheme = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Subject = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                    SessionId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    AuthenticationDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    State = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Realm = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsClientsNotified = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    SerializedClientIds = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "PresentationDefinitionFormat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Format = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ProofType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PresentationDefinitionInputDescriptorId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionFormat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionFormat_PresentationDefinitionInputDescriptor_PresentationDefinitionInputDescriptorId",
                        column: x => x.PresentationDefinitionInputDescriptorId,
                        principalTable: "PresentationDefinitionInputDescriptor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresentationDefinitionInputDescriptorConstraint",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Path = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Filter = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PresentationDefinitionInputDescriptorId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentationDefinitionInputDescriptorConstraint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresentationDefinitionInputDescriptorConstraint_PresentationDefinitionInputDescriptor_PresentationDefinitionInputDescriptorId",
                        column: x => x.PresentationDefinitionInputDescriptorId,
                        principalTable: "PresentationDefinitionInputDescriptor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationContextClassReferenceRealm",
                columns: table => new
                {
                    AuthenticationContextClassReferencesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ClientId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ClientSecret = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RegistrationAccessToken = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    GrantTypes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RedirectionUrls = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TokenEndPointAuthMethod = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ResponseTypes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    JwksUri = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Contacts = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SoftwareId = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SoftwareVersion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TlsClientAuthSubjectDN = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TlsClientAuthSanDNS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TlsClientAuthSanURI = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TlsClientAuthSanIP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TlsClientAuthSanEmail = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClientSecretExpirationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IsTokenExchangeEnabled = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    TokenExchangeType = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TokenExpirationTimeInSeconds = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    CNonceExpirationTimeInSeconds = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    RefreshTokenExpirationTimeInSeconds = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    TokenSignedResponseAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TokenEncryptedResponseAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TokenEncryptedResponseEnc = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PostLogoutRedirectUris = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    RedirectToRevokeSessionUI = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    PreferredTokenProfile = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequestObjectSigningAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequestObjectEncryptionAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequestObjectEncryptionEnc = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SubjectType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PairWiseIdentifierSalt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SectorIdentifierUri = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IdTokenSignedResponseAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IdTokenEncryptedResponseAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IdTokenEncryptedResponseEnc = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BCTokenDeliveryMode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BCClientNotificationEndpoint = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BCAuthenticationRequestSigningAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserInfoSignedResponseAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserInfoEncryptedResponseAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UserInfoEncryptedResponseEnc = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BCUserCodeParameter = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    FrontChannelLogoutUri = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CredentialOfferEndpoint = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsTransactionCodeRequired = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    PreAuthCodeExpirationTimeInSeconds = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    FrontChannelLogoutSessionRequired = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    BackChannelLogoutSessionRequired = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    DefaultMaxAge = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    TlsClientCertificateBoundAccessToken = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    BackChannelLogoutUri = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ApplicationType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    InitiateLoginUri = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RequireAuthTime = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    AuthorizationSignedResponseAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AuthorizationEncryptedResponseAlg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AuthorizationDataTypes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AuthorizationEncryptedResponseEnc = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DPOPBoundAccessTokens = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    IsConsentDisabled = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    IsResourceParameterRequired = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    AuthReqIdExpirationTimeInSeconds = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ClientType = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IsDPOPNonceRequired = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    DPOPNonceLifetimeInSeconds = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    IsRedirectUrlCaseSensitive = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    SerializedParameters = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DefaultAcrValues = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    BCIntervalSeconds = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AccessTokenType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AuthenticationContextClassReferenceId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Acrs_AuthenticationContextClassReferenceId",
                        column: x => x.AuthenticationContextClassReferenceId,
                        principalTable: "Acrs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuthorizedScope",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Scope = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ConsentId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "ClientJsonWebKey",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Kid = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Alg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Usage = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    KeyType = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    SerializedJsonWebKey = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ClientId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                name: "ClientRealm",
                columns: table => new
                {
                    ClientsId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RealmsName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "ClientScope",
                columns: table => new
                {
                    ClientsId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ScopesId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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
                name: "DeviceAuthCodes",
                columns: table => new
                {
                    DeviceCode = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UserCode = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ClientId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    UserLogin = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Scopes = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ExpirationDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    NextAccessDateTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LastAccessTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Key = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Value = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Language = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ClientId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
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
                name: "AuthorizedResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Resource = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Audience = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AuthorizedScopeId = table.Column<int>(type: "NUMBER(10)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ConfigurationDefinitionRecordTranslation",
                columns: table => new
                {
                    ConfigurationDefinitionRecordId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    TranslationsId = table.Column<int>(type: "NUMBER(10)", nullable: false)
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
                name: "ConfigurationDefinitionRecordValueTranslation",
                columns: table => new
                {
                    ConfigurationDefinitionRecordValueId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    TranslationsId = table.Column<int>(type: "NUMBER(10)", nullable: false)
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
                name: "TranslationUMAResource",
                columns: table => new
                {
                    TranslationsId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UMAResourceId = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Acrs_RegistrationWorkflowId",
                table: "Acrs",
                column: "RegistrationWorkflowId");

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
                name: "IX_Clients_AuthenticationContextClassReferenceId",
                table: "Clients",
                column: "AuthenticationContextClassReferenceId");

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
                name: "IX_PresentationDefinitionFormat_PresentationDefinitionInputDescriptorId",
                table: "PresentationDefinitionFormat",
                column: "PresentationDefinitionInputDescriptorId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitionInputDescriptor_PresentationDefinitionId",
                table: "PresentationDefinitionInputDescriptor",
                column: "PresentationDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitionInputDescriptorConstraint_PresentationDefinitionInputDescriptorId",
                table: "PresentationDefinitionInputDescriptorConstraint",
                column: "PresentationDefinitionInputDescriptorId");

            migrationBuilder.CreateIndex(
                name: "IX_PresentationDefinitions_RealmName",
                table: "PresentationDefinitions",
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
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "DeviceAuthCodes");

            migrationBuilder.DropTable(
                name: "ExtractedRepresentations");

            migrationBuilder.DropTable(
                name: "ExtractedRepresentationsStaging");

            migrationBuilder.DropTable(
                name: "GotifySessions");

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
                name: "Languages");

            migrationBuilder.DropTable(
                name: "MessageBusErrorMessages");

            migrationBuilder.DropTable(
                name: "PresentationDefinitionFormat");

            migrationBuilder.DropTable(
                name: "PresentationDefinitionInputDescriptorConstraint");

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
                name: "Groups");

            migrationBuilder.DropTable(
                name: "PresentationDefinitionInputDescriptor");

            migrationBuilder.DropTable(
                name: "SerializedFileKeys");

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
                name: "PresentationDefinitions");

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
                name: "Acrs");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningLst");

            migrationBuilder.DropTable(
                name: "RegistrationWorkflows");

            migrationBuilder.DropTable(
                name: "IdentityProvisioningDefinitions");

            migrationBuilder.DropTable(
                name: "Realms");
        }
    }
}
