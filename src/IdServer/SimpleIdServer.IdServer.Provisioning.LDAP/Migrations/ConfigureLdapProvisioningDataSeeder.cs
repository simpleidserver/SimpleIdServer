// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DataSeeder;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Provisioning.LDAP.Migrations;

public class ConfigureLdapProvisioningDataSeeder : BaseProvisioningDataseeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public ConfigureLdapProvisioningDataSeeder(ITransactionBuilder transactionBuilder, IIdentityProvisioningStore identityProvisioningStore, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository, IRealmRepository realmRepository) : base(identityProvisioningStore, realmRepository, dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(ConfigureLdapProvisioningDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            LdapDefinition.Instances = new List<IdentityProvisioning>
            {
                LdapInstance
            };
            await TryAddProvisioningDef(LdapDefinition, cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }

    private static IdentityProvisioning LdapInstance = IdentityProvisioningBuilder.Create(LdapDefinition, "LDAP", "LDAP", "LDAP").Build();

    private static IdentityProvisioningDefinition LdapDefinition = IdentityProvisioningDefinitionBuilder.Create<LDAPRepresentationsExtractionJobOptions>(SimpleIdServer.IdServer.Provisioning.LDAP.Services.LDAPProvisioningService.NAME, "LDAP")
        .AddUserSubjectMappingRule("cn")
        .AddGroupNameMappingRule("cn")
        .AddLDAPDistinguishedName()
        .Build();
}
