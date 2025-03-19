// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using DataSeeder;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Conf.Migrations.AfterDeployment;

// TODO : Move to a configuration file !!
public class ConfigureIdProvisioningDataSeeder : BaseAfterDeploymentDataSeeder
{
    public ConfigureIdProvisioningDataSeeder(IDataSeederExecutionHistoryRepository dataSeederExecutionHistoryRepository) : base(dataSeederExecutionHistoryRepository)
    {
    }

    public override string Name => nameof(ConfigureIdProvisioningDataSeeder);

    protected override Task Execute(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }


    private static ICollection<IdentityProvisioningDefinition> IdentityProvisioningDefLst = new List<IdentityProvisioningDefinition>
    {
        Scim
    };

    private static ICollection<IdentityProvisioning> GetIdentityProvisiongLst(string scimEdp) => new List<IdentityProvisioning>
    {
        IdentityProvisioningBuilder.Create(Scim, "SCIM", "SCIM").Build(),
        IdentityProvisioningBuilder.Create(Ldap, "LDAP", "LDAP", "LDAP").Build()
    };

    private static IdentityProvisioningDefinition Ldap = IdentityProvisioningDefinitionBuilder.Create<LDAPRepresentationsExtractionJobOptions>(SimpleIdServer.IdServer.Provisioning.LDAP.Services.LDAPProvisioningService.NAME, "LDAP")
        .AddUserSubjectMappingRule("cn")
        .AddGroupNameMappingRule("cn")
        .AddLDAPDistinguishedName()
        .Build();

    private static IdentityProvisioningDefinition Scim = IdentityProvisioningDefinitionBuilder.Create<SCIMRepresentationsExtractionJobOptions>(SimpleIdServer.IdServer.Provisioning.SCIM.Services.SCIMProvisioningService.NAME, "SCIM")
        .AddUserSubjectMappingRule("$.userName")
        .AddUserPropertyMappingRule("$.name.familyName", nameof(User.Lastname))
        .AddUserAttributeMappingRule("$.name.givenName", JwtRegisteredClaimNames.GivenName)
        .AddGroupNameMappingRule("$.displayName")
        .Build();
}
