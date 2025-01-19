// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.EF;
using FormBuilder.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Notification.Gotify;
using SimpleIdServer.IdServer.Otp;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.Startup.Converters;
using SimpleIdServer.IdServer.Store.EF;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.VerifiablePresentation;
using SimpleIdServer.IdServer.WsFederation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleIdServer.IdServer.Startup.Conf;

public class DataSeeder
{
    private static List<FormRecord> allAuthForms = new List<FormRecord>
    {
        StandardConsoleAuthForms.ConsoleForm,
        StandardEmailAuthForms.EmailForm,
        StandardFidoAuthForms.WebauthnForm,
        StandardFidoAuthForms.MobileForm,
        StandardOtpAuthForms.OtpForm,
        StandardPwdAuthForms.PwdForm,
        StandardPwdAuthForms.ResetForm,
        StandardPwdAuthForms.ConfirmResetForm,
        StandardSmsAuthForms.SmsForm
    };

    private static List<FormRecord> allRegForms = new List<FormRecord>
    {
        StandardEmailRegistrationForms.EmailForm,
        StandardFidoRegisterForms.WebauthnForm,
        StandardFidoRegisterForms.MobileForm,
        StandardPwdRegisterForms.PwdForm,
        StandardSmsRegisterForms.SmsForm,
        StandardVpRegisterForms.VpForm
    };

    private static List<WorkflowRecord> allAuthWorkflows = new List<WorkflowRecord>
    {
        StandardConsoleAuthWorkflows.DefaultWorkflow,
        StandardEmailAuthWorkflows.DefaultWorkflow,
        StandardFidoAuthWorkflows.DefaultWebauthnWorkflow,
        StandardFidoAuthWorkflows.DefaultMobileWorkflow,
        StandardOtpAuthWorkflows.DefaultWorkflow,
        StandardPwdAuthWorkflows.DefaultPwdWorkflow,
        StandardPwdAuthWorkflows.DefaultConfirmResetPwdWorkflow,
        StandardSmsAuthWorkflows.DefaultWorkflow
    };

    public static List<WorkflowRecord> allRegistrationWorkflows = new List<WorkflowRecord>
    {
        StandardEmailRegisterWorkflows.DefaultWorkflow,
        StandardFidoRegistrationWorkflows.WebauthnWorkflow,
        StandardFidoRegistrationWorkflows.MobileWorkflow,
        StandardPwdRegistrationWorkflows.DefaultWorkflow,
        StandardSmsRegisterWorkflows.DefaultWorkflow,
        StandardVpRegistrationWorkflows.DefaultWorkflow
    };

    public static void SeedData(WebApplication webApplication, string scimBaseUrl)
    {
        using (var scope = webApplication.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            SeedSid(scope, scimBaseUrl);
            SeedFormRecords(scope);
        }
    }

    private static void SeedSid(IServiceScope scope, string scimBaseUrl)
    {
        using (var dbContext = scope.ServiceProvider.GetService<StoreDbContext>())
        {
            var isInMemory = dbContext.Database.IsInMemory();
            if (!isInMemory) dbContext.Database.Migrate();
            var masterRealm = SeedRealms(dbContext);
            var unsupportedScopes = SeedScopes(dbContext, masterRealm);
            SeedClients(dbContext, unsupportedScopes, masterRealm);
            SeedUmaPendings(dbContext);
            SeedUmaResources(dbContext);
            SeedGotifySessions(dbContext);
            SeedLanguages(dbContext);
            SeedAuthSchemes(dbContext);
            SeedIdProvisioning(dbContext, scimBaseUrl);
            SeedRegistrationWorkflows(dbContext);
            var groups = SeedGroups(dbContext, masterRealm);
            SeedUsers(dbContext, groups.adminGroup, groups.adminRoGroup, groups.fastFedGroup);
            SeedSerializedFileKeys(dbContext);
            SeedCertificateAuthorities(dbContext);
            SeedPresentationDefinitions(dbContext);
            SeedFederationEntities(dbContext);
            SeedAcrs(dbContext);
            MigrationService.EnableIsolationLevel(dbContext);
            dbContext.SaveChanges();
        }
    }

    private static Realm SeedRealms(StoreDbContext dbContext)
    {
        var masterRealm = dbContext.Realms.FirstOrDefault(r => r.Name == SimpleIdServer.IdServer.Constants.StandardRealms.Master.Name) ?? SimpleIdServer.IdServer.Constants.StandardRealms.Master;
        if (!dbContext.Realms.Any())
            dbContext.Realms.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Realms);
        return masterRealm;
    }

    private static List<Scope> SeedScopes(StoreDbContext dbContext, Realm masterRealm)
    {
        var allScopeNames = dbContext.Scopes.Select(s => s.Name);
        var unsupportedScopes = SimpleIdServer.IdServer.Startup.IdServerConfiguration.Scopes.Where(s => !allScopeNames.Contains(s.Name));
        foreach (var scope in unsupportedScopes)
            scope.Realms = new List<Realm>
                {
                    masterRealm
                };
        dbContext.Scopes.AddRange(unsupportedScopes);
        return unsupportedScopes.ToList();
    }

    private static void SeedClients(StoreDbContext dbContext, List<Scope> unsupportedScopes, Realm masterRealm)
    {
        var confClientIds = SimpleIdServer.IdServer.Startup.IdServerConfiguration.Clients.Select(c => c.ClientId);
        var allClientIds = dbContext.Clients.Select(s => s.ClientId);
        var unknownClients = SimpleIdServer.IdServer.Startup.IdServerConfiguration.Clients.Where(c => !allClientIds.Contains(c.ClientId));
        var knownClients = dbContext.Clients
            .Include(c => c.Scopes)
            .Where(c => confClientIds.Contains(c.ClientId));
        foreach (var unknownClient in unknownClients)
        {
            unknownClient.Realms = new List<Realm>
                {
                    masterRealm
                };
            var scopeNames = unknownClient.Scopes.Select(s => s.Name).ToList();
            unknownClient.Scopes.Clear();
            foreach (var name in scopeNames)
            {
                var existingScope = dbContext.Scopes.SingleOrDefault(s => s.Name == name) ?? unsupportedScopes.Single(s => s.Name == name);
                unknownClient.Scopes.Add(existingScope);
            }

            dbContext.Clients.Add(unknownClient);
        }

        foreach (var knownClient in knownClients)
        {
            var cl = SimpleIdServer.IdServer.Startup.IdServerConfiguration.Clients.Single(c => c.ClientId == knownClient.ClientId);
            foreach (var scope in cl.Scopes)
            {
                if (!knownClient.Scopes.Any(s => s.Name == scope.Name))
                {
                    var existingScope = dbContext.Scopes.SingleOrDefault(s => s.Name == scope.Name) ?? unsupportedScopes.Single(s => s.Name == scope.Name);
                    knownClient.Scopes.Add(existingScope);
                }
            }
        }
    }

    private static void SeedUmaPendings(StoreDbContext dbContext)
    {
        if (!dbContext.UmaPendingRequest.Any())
            dbContext.UmaPendingRequest.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.PendingRequests);
    }

    private static void SeedUmaResources(StoreDbContext dbContext)
    {
        if (!dbContext.UmaResources.Any())
            dbContext.UmaResources.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Resources);
    }

    private static void SeedGotifySessions(StoreDbContext dbContext)
    {
        if (!dbContext.GotifySessions.Any())
            dbContext.GotifySessions.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.Sessions);
    }

    private static void SeedLanguages(StoreDbContext dbContext)
    {
        foreach (var language in SimpleIdServer.IdServer.Startup.IdServerConfiguration.Languages)
        {
            if (!dbContext.Languages.Any(l => l.Code == language.Code))
                dbContext.Languages.Add(language);
            var keys = language.Descriptions.Select(d => d.Key);
            var existingTranslations = dbContext.Translations.Where(t => keys.Contains(t.Key)).ToList();
            var unknownTranslations = language.Descriptions.Where(d => !existingTranslations.Any(t => t.Key == d.Key && t.Language == d.Language));
            dbContext.Translations.AddRange(unknownTranslations);
            foreach (var existingTranslation in existingTranslations)
            {
                var tr = language.Descriptions.SingleOrDefault(d => d.Key == existingTranslation.Key && d.Language == existingTranslation.Language);
                if (tr == null) continue;
                existingTranslation.Value = tr.Value;
            }
        }
    }

    private static void SeedAuthSchemes(StoreDbContext dbContext)
    {
        foreach (var providerDefinition in SimpleIdServer.IdServer.Startup.IdServerConfiguration.ProviderDefinitions)
        {
            if (!dbContext.AuthenticationSchemeProviderDefinitions.Any(d => d.Name == providerDefinition.Name))
            {
                dbContext.AuthenticationSchemeProviderDefinitions.Add(providerDefinition);
            }
        }

        if (!dbContext.AuthenticationSchemeProviders.Any())
        {
            foreach (var provider in SimpleIdServer.IdServer.Startup.IdServerConfiguration.Providers)
            {
                var def = dbContext.AuthenticationSchemeProviderDefinitions.FirstOrDefault(d => d.Name == provider.AuthSchemeProviderDefinition.Name);
                if (def != null) provider.AuthSchemeProviderDefinition = def;
                var realmName = provider.Realms.First().Name;
                var realm = dbContext.Realms.FirstOrDefault(r => r.Name == realmName);
                if (realm != null)
                {
                    provider.Realms.Clear();
                    provider.Realms.Add(realm);
                }

                dbContext.AuthenticationSchemeProviders.Add(provider);
            }
        }
    }

    private static void SeedIdProvisioning(StoreDbContext dbContext, string scimBaseUrl)
    {
        if (!dbContext.IdentityProvisioningDefinitions.Any())
            dbContext.IdentityProvisioningDefinitions.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.IdentityProvisioningDefLst);

        if (!dbContext.IdentityProvisioningLst.Any())
            dbContext.IdentityProvisioningLst.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.GetIdentityProvisiongLst(scimBaseUrl));
    }

    private static void SeedRegistrationWorkflows(StoreDbContext dbContext)
    {
        if (!dbContext.RegistrationWorkflows.Any())
            dbContext.RegistrationWorkflows.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.RegistrationWorkflows);
    }

    private static (Group adminGroup, Group adminRoGroup, Group fastFedGroup) SeedGroups(StoreDbContext dbContext, Realm masterRealm)
    {
        var admGroup = dbContext.Groups.Include(g => g.Realms)
            .Include(g => g.Roles)
            .FirstOrDefault(g => g.Name == SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorGroup.Name);
        var admRoGroup = dbContext.Groups.Include(g => g.Realms)
            .Include(g => g.Roles)
            .FirstOrDefault(g => g.Name == SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorReadonlyGroup.Name);
        var fastFedAdmGroup = dbContext.Groups.Include(g => g.Realms)
            .Include(g => g.Roles)
            .FirstOrDefault(g => g.Name == IdServerConfiguration.FastFedAdministratorGroup.Name);

        if (admGroup == null)
        {
            admGroup = SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorGroup;
            admGroup.Realms = new List<GroupRealm>();
            masterRealm.Groups.Add(new GroupRealm
            {
                Group = admGroup
            });
        }

        if (admRoGroup == null)
        {
            admRoGroup = SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorReadonlyGroup;
            admRoGroup.Realms = new List<GroupRealm>();
            masterRealm.Groups.Add(new GroupRealm
            {
                Group = admRoGroup
            });
        }

        var scopes = RealmRoleBuilder.BuildAdministrativeRole(masterRealm);
        var allScopeNames = dbContext.Scopes.Select(s => s.Name);
        var unknownScopes = scopes.Where(s => !allScopeNames.Contains(s.Name));
        dbContext.Scopes.AddRange(unknownScopes);
        foreach (var scope in unknownScopes)
        {
            if (!admGroup.Roles.Any(r => r.Name == scope.Name))
                admGroup.Roles.Add(scope);
        }

        foreach (var scope in unknownScopes.Where(s => s.Action == ComponentActions.View))
        {
            if (!admRoGroup.Roles.Any(r => r.Name == scope.Name))
                admRoGroup.Roles.Add(scope);
        }

        var existingAdministratorRole = dbContext.Scopes.FirstOrDefault(s => s.Name == SimpleIdServer.IdServer.Constants.StandardScopes.WebsiteAdministratorRole.Name);
        if (existingAdministratorRole == null)
        {
            existingAdministratorRole = SimpleIdServer.IdServer.Constants.StandardScopes.WebsiteAdministratorRole;
            existingAdministratorRole.Realms.Clear();
            existingAdministratorRole.Realms.Add(masterRealm);
            dbContext.Scopes.Add(existingAdministratorRole);
        }

        if (!admGroup.Roles.Any(r => r.Name == SimpleIdServer.IdServer.Constants.StandardScopes.WebsiteAdministratorRole.Name))
            admGroup.Roles.Add(existingAdministratorRole);

        if (fastFedAdmGroup == null)
        {
            var grp = IdServerConfiguration.FastFedAdministratorGroup;
            foreach (var role in grp.Roles)
            {
                role.Realms = new List<Realm>
                    {
                        masterRealm
                    };
            }

            dbContext.Groups.Add(grp);
            fastFedAdmGroup = grp;
        }

        return (admGroup, admRoGroup, fastFedAdmGroup);
    }

    private static void SeedUsers(StoreDbContext dbContext, Group adminGroup, Group adminRoGroup, Group fastFedGroup)
    {
        var isUserExists = dbContext.Users
            .Any(c => c.Name == "user");
        var existingAdministratorUser = dbContext.Users
            .Include(u => u.Groups).ThenInclude(u => u.Group)
            .FirstOrDefault(u => u.Name == SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorUser.Name);
        var existingAdministratorRoUser = dbContext.Users
            .Include(u => u.Groups).ThenInclude(u => u.Group)
            .FirstOrDefault(u => u.Name == SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorReadonlyUser.Name);
        if (!isUserExists)
            dbContext.Users.Add(UserBuilder.Create("user", "password", "User").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").Build());
        if (existingAdministratorRoUser == null)
            dbContext.Users.Add(SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorReadonlyUser);
        else if (!existingAdministratorRoUser.Groups.Any(g => g.Group.Name == SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorReadonlyGroup.Name))
        {
            existingAdministratorRoUser.Groups.Add(new GroupUser
            {
                Group = adminRoGroup
            });
        }

        if (existingAdministratorUser == null)
        {
            var user = SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorUser;
            user.Groups.Add(new GroupUser
            {
                Group = fastFedGroup
            });
            dbContext.Users.Add(user);
        }
        else
        {
            if (!existingAdministratorUser.Groups.Any(g => g.Group.Name == SimpleIdServer.IdServer.Constants.StandardGroups.AdministratorGroup.Name))
            {
                existingAdministratorUser.Groups.Add(new GroupUser
                {
                    Group = adminGroup
                });
            }

            if (!existingAdministratorUser.Groups.Any(g => g.Group.Name == IdServerConfiguration.FastFedAdministratorGroup.Name))
            {
                existingAdministratorUser.Groups.Add(new GroupUser
                {
                    Group = fastFedGroup
                });
            }
        }
    }

    private static void SeedSerializedFileKeys(StoreDbContext dbContext)
    {
        if (!dbContext.SerializedFileKeys.Any())
        {
            dbContext.SerializedFileKeys.AddRange(SimpleIdServer.IdServer.Constants.StandardKeys);
            dbContext.SerializedFileKeys.Add(WsFederationKeyGenerator.GenerateWsFederationSigningCredentials(SimpleIdServer.IdServer.Constants.StandardRealms.Master));
        }
    }

    private static void SeedCertificateAuthorities(StoreDbContext dbContext)
    {
        if (!dbContext.CertificateAuthorities.Any())
            dbContext.CertificateAuthorities.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.CertificateAuthorities);
    }

    private static void SeedPresentationDefinitions(StoreDbContext dbContext)
    {
        if (!dbContext.PresentationDefinitions.Any())
            dbContext.PresentationDefinitions.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.PresentationDefinitions);
    }

    private static void SeedFederationEntities(StoreDbContext dbContext)
    {
        if (!dbContext.FederationEntities.Any())
            dbContext.FederationEntities.AddRange(SimpleIdServer.IdServer.Startup.IdServerConfiguration.FederationEntities);
    }

    private static void SeedAcrs(StoreDbContext dbContext)
    {
        if (!dbContext.Acrs.Any())
        {
            var firstLevelAssurance = SimpleIdServer.IdServer.Constants.StandardAcrs.FirstLevelAssurance;
            var iapSilver = SimpleIdServer.IdServer.Constants.StandardAcrs.IapSilver;
            firstLevelAssurance.AuthenticationWorkflow = StandardPwdAuthWorkflows.pwdWorkflowId;
            iapSilver.AuthenticationWorkflow = StandardPwdAuthWorkflows.pwdWorkflowId;
            dbContext.Acrs.Add(firstLevelAssurance);
            dbContext.Acrs.Add(iapSilver);
            dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
            {
                Id = Guid.NewGuid().ToString(),
                Name = "email",
                DisplayName = "Email authentication",
                UpdateDateTime = DateTime.UtcNow,
                Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
            });
            dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
            {
                Id = Guid.NewGuid().ToString(),
                Name = "sms",
                DisplayName = "Sms authentication",
                UpdateDateTime = DateTime.UtcNow,
                Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
            });
            dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
            {
                Id = Guid.NewGuid().ToString(),
                Name = "pwd-email",
                DisplayName = "Password and email authentication",
                UpdateDateTime = DateTime.UtcNow,
                Realms = new List<Realm>
                    {
                        SimpleIdServer.IdServer.Constants.StandardRealms.Master
                    }
            });
        }
    }

    private static void SeedConfigurations(StoreDbContext dbContext)
    {
        AddMissingConfigurationDefinition<FacebookOptionsLite>(dbContext);
        AddMissingConfigurationDefinition<LDAPRepresentationsExtractionJobOptions>(dbContext);
        AddMissingConfigurationDefinition<SCIMRepresentationsExtractionJobOptions>(dbContext);
        AddMissingConfigurationDefinition<IdServerEmailOptions>(dbContext);
        AddMissingConfigurationDefinition<IdServerSmsOptions>(dbContext);
        AddMissingConfigurationDefinition<IdServerPasswordOptions>(dbContext);
        AddMissingConfigurationDefinition<IdServerVpOptions>(dbContext);
        AddMissingConfigurationDefinition<WebauthnOptions>(dbContext);
        AddMissingConfigurationDefinition<MobileOptions>(dbContext);
        AddMissingConfigurationDefinition<IdServerConsoleOptions>(dbContext);
        AddMissingConfigurationDefinition<GotifyOptions>(dbContext);
        AddMissingConfigurationDefinition<SimpleIdServer.IdServer.Notification.Fcm.FcmOptions>(dbContext);
        AddMissingConfigurationDefinition<GoogleOptionsLite>(dbContext);
        AddMissingConfigurationDefinition<NegotiateOptionsLite>(dbContext);
        AddMissingConfigurationDefinition<UserLockingOptions>(dbContext);

        void AddMissingConfigurationDefinition<T>(StoreDbContext dbContext)
        {
            var name = typeof(T).Name;
            if (!dbContext.Definitions.Any(d => d.Id == name))
            {
                dbContext.Definitions.Add(ConfigurationDefinitionExtractor.Extract<T>());
            }
        }
    }

    private static void SeedFormRecords(IServiceScope scope)
    {
        using (var formDbContext = scope.ServiceProvider.GetService<FormBuilderDbContext>())
        {
            var isInMemory = formDbContext.Database.IsInMemory();
            if (!isInMemory) formDbContext.Database.Migrate();
            var allForms = new List<FormRecord>();
            allForms.AddRange(allAuthForms);
            allForms.AddRange(allRegForms);
            var content = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "form.css"));
            foreach (var form in allForms)
            {
                form.AvailableStyles.Add(new FormBuilder.Models.FormStyle
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = content,
                    IsActive = true
                });
            }

            formDbContext.Forms.AddRange(allForms);
            formDbContext.Workflows.AddRange(allAuthWorkflows);
            formDbContext.Workflows.AddRange(allRegistrationWorkflows);
            formDbContext.SaveChanges();
        }
    }
}
