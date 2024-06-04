// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class DbContext : IDisposable
{
    private readonly SqlSugarScope _client;

    public DbContext(IOptions<SqlSugarOptions> options)
    {
        var connectionConfig = options.Value.ConnectionConfig;
        connectionConfig.IsAutoCloseConnection = true;
        connectionConfig.DbType = options.Value.DbType;
        _client = new SqlSugarScope(connectionConfig);
        UserSessions = new SimpleClient<SugarUserSession>(_client);
        Users = new SimpleClient<SugarUser>();
    }

    public SimpleClient<SugarUserSession> UserSessions { get; set; }
    public SimpleClient<SugarUser> Users { get; set; }
    public SqlSugarScope SqlSugarClient
    {
        get
        {
            return _client;
        }
    }

    public void Migrate()
    {
        _client.DbMaintenance.CreateDatabase();
        _client.CodeFirst.InitTables<SugarApiResource>();
        _client.CodeFirst.InitTables<SugarApiResourceRealm>();
        _client.CodeFirst.InitTables<SugarApiResourceScope>();
        _client.CodeFirst.InitTables<SugarAuditEvent>();
        _client.CodeFirst.InitTables<SugarAuthenticationContextClassReference>();
        _client.CodeFirst.InitTables<SugarAuthenticationContextClassReferenceRealm>();
        _client.CodeFirst.InitTables<SugarAuthenticationSchemeProvider>();
        _client.CodeFirst.InitTables<SugarAuthenticationSchemeProviderDefinition>();
        _client.CodeFirst.InitTables<SugarAuthenticationSchemeProviderMapper>();
        _client.CodeFirst.InitTables<SugarAuthenticationSchemeProviderRealm>();
        _client.CodeFirst.InitTables<SugarAuthorizedResource>();
        _client.CodeFirst.InitTables<SugarAuthorizedScope>();
        _client.CodeFirst.InitTables<SugarBCAuthorize>();
        _client.CodeFirst.InitTables<SugarBCAuthorizeHistory>();
        _client.CodeFirst.InitTables<SugarCertificateAuthority>();
        _client.CodeFirst.InitTables<SugarCertificateAuthorityRealm>();
        _client.CodeFirst.InitTables<SugarClaimProvider>();
        _client.CodeFirst.InitTables<SugarClient>();
        _client.CodeFirst.InitTables<SugarClientCertificate>();
        _client.CodeFirst.InitTables<SugarClientJsonWebKey>();
        _client.CodeFirst.InitTables<SugarClientRealm>();
        _client.CodeFirst.InitTables<SugarClientScope>();
        _client.CodeFirst.InitTables<SugarConfigurationDefinition>();
        _client.CodeFirst.InitTables<SugarConfigurationDefinitionRecord>();
        _client.CodeFirst.InitTables<SugarConfigurationDefinitionRecordTranslation>();
        _client.CodeFirst.InitTables<SugarConfigurationDefinitionRecordValue>();
        _client.CodeFirst.InitTables<SugarConfigurationDefinitionRecordValueTranslation>();
        _client.CodeFirst.InitTables<SugarConfigurationKeyPairValueRecord>();
        _client.CodeFirst.InitTables<SugarConsent>();
        _client.CodeFirst.InitTables<SugarDataProtection>();
        _client.CodeFirst.InitTables<SugarDeviceAuthCode>();
        _client.CodeFirst.InitTables<SugarExtractedRepresentation>();
        _client.CodeFirst.InitTables<SugarExtractedRepresentationStaging>();
        _client.CodeFirst.InitTables<SugarGotifySession>();
        _client.CodeFirst.InitTables<SugarGroup>();
        _client.CodeFirst.InitTables<SugarGroupRealm>();
        _client.CodeFirst.InitTables<SugarGroupScope>();
        _client.CodeFirst.InitTables<SugarGroupUser>();
        _client.CodeFirst.InitTables<SugarIdentityProvisioning>();
        _client.CodeFirst.InitTables<SugarIdentityProvisioningDefinition>();
        _client.CodeFirst.InitTables<SugarIdentityProvisioningMappingRule>();
        _client.CodeFirst.InitTables<SugarIdentityProvisioningRealm>();
        _client.CodeFirst.InitTables<SugarLanguage>();
        _client.CodeFirst.InitTables<SugarMessageBusErrorMessage>();
        _client.CodeFirst.InitTables<SugarPresentationDefinition>();
        _client.CodeFirst.InitTables<SugarPresentationDefinitionFormat>();
        _client.CodeFirst.InitTables<SugarPresentationDefinitionInputDescriptor>();
        _client.CodeFirst.InitTables<SugarPresentationDefinitionInputDescriptorConstraint>();
        _client.CodeFirst.InitTables<SugarRealm>();
        _client.CodeFirst.InitTables<SugarRealmSerializedFileKey>();
        _client.CodeFirst.InitTables<SugarRealmUser>();
        _client.CodeFirst.InitTables<SugarRegistrationWorkflow>();
        _client.CodeFirst.InitTables<SugarScope>();
        _client.CodeFirst.InitTables<SugarScopeClaimMapper>();
        _client.CodeFirst.InitTables<SugarScopeRealm>();
        _client.CodeFirst.InitTables<SugarSerializedFileKey>();
        _client.CodeFirst.InitTables<SugarToken>();
        _client.CodeFirst.InitTables<SugarTranslation>();
        _client.CodeFirst.InitTables<SugarTranslationUMAResource>();
        _client.CodeFirst.InitTables<SugarUMAPendingRequest>();
        _client.CodeFirst.InitTables<SugarUMAResource>();
        _client.CodeFirst.InitTables<SugarUMAResourcePermission>();
        _client.CodeFirst.InitTables<SugarUMAResourcePermissionClaim>();
        _client.CodeFirst.InitTables<SugarUser>();
        _client.CodeFirst.InitTables<SugarUserClaim>();
        _client.CodeFirst.InitTables<SugarUserCredential>();
        _client.CodeFirst.InitTables<SugarUserDevice>();
        _client.CodeFirst.InitTables<SugarUserExternalAuthProvider>();
        _client.CodeFirst.InitTables<SugarUserSession>();
    }

    public SqlSugarScope Client
    {
        get
        {
            return _client;
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public Guid Id { get; set; }
}
