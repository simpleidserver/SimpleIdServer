// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Migrations;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Provisioning.SCIM.Migrations;

public class ConfigureScimProvisioningDataSeeder : BaseProvisioningDataseeder
{
    private readonly ITransactionBuilder _transactionBuilder;

    public ConfigureScimProvisioningDataSeeder(ITransactionBuilder transactionBuilder, IIdentityProvisioningStore identityProvisioningStore, IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository, IRealmRepository realmRepository) : base(identityProvisioningStore, realmRepository, dataSeederExecutionHistoryRepository)
    {
        _transactionBuilder = transactionBuilder;
    }

    public override string Name => nameof(ConfigureScimProvisioningDataSeeder);

    protected override async Task Execute(CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            ScimDefinition.Instances = new List<IdentityProvisioning>
            {
                ScimInstance
            };
            await TryAddProvisioningDef(ScimDefinition, cancellationToken);
            await transaction.Commit(cancellationToken);
        }
    }

    private static IdentityProvisioning ScimInstance = IdentityProvisioningBuilder.Create(ScimDefinition, "SCIM", "SCIM").Build();

    private static IdentityProvisioningDefinition ScimDefinition = IdentityProvisioningDefinitionBuilder.Create<SCIMRepresentationsExtractionJobOptions>(SimpleIdServer.IdServer.Provisioning.SCIM.Services.SCIMProvisioningService.NAME, "SCIM")
        .AddUserSubjectMappingRule("$.userName")
        .AddUserPropertyMappingRule("$.name.familyName", nameof(User.Lastname))
        .AddUserAttributeMappingRule("$.name.givenName", JwtRegisteredClaimNames.GivenName)
        .AddGroupNameMappingRule("$.displayName")
        .Build();
}
