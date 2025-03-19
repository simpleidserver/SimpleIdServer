﻿// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.EF;
using FormBuilder.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Configuration;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SimpleIdServer.IdServer.Startup.Conf;

public class DataSeeder
{
    public static string completePwdAuthWorkflowId = "059f49b2-f76a-4b5a-8ecc-cf64abdf9b39";
    private static string pwdEmailAuthWorkflowId = "ddeb825e-5012-4e7a-8441-d3eec73083c2";

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
        StandardSmsAuthWorkflows.DefaultWorkflow,
        BuildCompletePwdAuthWorkflow(),
        BuildPwdEmailAuthWorkflow()
    };

    public static List<WorkflowRecord> allRegistrationWorkflows = new List<WorkflowRecord>
    {
        StandardEmailRegisterWorkflows.DefaultWorkflow,
        StandardFidoRegistrationWorkflows.WebauthnWorkflow,
        StandardFidoRegistrationWorkflows.MobileWorkflow,
        StandardPwdRegistrationWorkflows.DefaultWorkflow,
        StandardSmsRegisterWorkflows.DefaultWorkflow,
        StandardVpRegistrationWorkflows.DefaultWorkflow,
        BuildComplexRegistrationWorkflow()
    };
    
    public static WorkflowRecord BuildComplexRegistrationWorkflow() => WorkflowBuilder.New("327dfdc9-3fa3-4b90-bfa8-670147fb4703")
          .AddPwdRegistration(StandardFidoRegisterForms.MobileForm)
          .AddMobileRegistration()
          .Build(DateTime.UtcNow);

    public static WorkflowRecord BuildCompletePwdAuthWorkflow() => WorkflowBuilder.New(completePwdAuthWorkflowId)
        .AddPwdAuth(resetStep: StandardPwdAuthForms.ResetForm)
        .AddResetPwd(StandardPwdAuthForms.ConfirmResetForm)
        .AddConfirmResetPwd()
        .Build(DateTime.UtcNow);

    public static WorkflowRecord BuildPwdEmailAuthWorkflow() => WorkflowBuilder.New(pwdEmailAuthWorkflowId)
        .AddPwdAuth(StandardEmailAuthForms.EmailForm, StandardPwdAuthForms.ResetForm)
        .AddEmailAuth()
        .AddResetPwd(StandardPwdAuthForms.ConfirmResetForm)
        .AddConfirmResetPwd()
        .Build(DateTime.UtcNow);

    public static void MigrateDataBeforeDeployment(WebApplication webApplication)
    {
        using (var scope = webApplication.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dataMigrationService = scope.ServiceProvider.GetService<IDataMigrationService>();
            dataMigrationService.MigrateBeforeDeployment(CancellationToken.None).Wait();
        }
    }

    public static void SeedData(WebApplication webApplication, string scimBaseUrl)
    {
        using (var scope = webApplication.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            SeedSid(scope, scimBaseUrl);
            SeedFormRecords(scope);
        }
    }

    public static void MigrateDataAfterDeployment(WebApplication webApplication)
    {
        using (var scope = webApplication.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dataMigrationService = scope.ServiceProvider.GetService<IDataMigrationService>();
            dataMigrationService.MigrateAfterDeployment(CancellationToken.None).Wait();
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
            SeedGotifySessions(dbContext);
            SeedAuthSchemes(dbContext);
            SeedIdProvisioning(dbContext, scimBaseUrl);
            SeedSerializedFileKeys(dbContext);
            SeedCertificateAuthorities(dbContext);
            SeedPresentationDefinitions(dbContext);
            SeedFederationEntities(dbContext);
            SeedConfigurations(dbContext);
            MigrationService.EnableIsolationLevel(dbContext);
            dbContext.SaveChanges();
        }
    }

    private static Realm SeedRealms(StoreDbContext dbContext)
    {
        var masterRealm = dbContext.Realms.FirstOrDefault(r => r.Name == SimpleIdServer.IdServer.Config.DefaultRealms.Master.Name) ?? SimpleIdServer.IdServer.Config.DefaultRealms.Master;
        if (!dbContext.Realms.Any())
            dbContext.Realms.AddRange(SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.Realms);
        return masterRealm;
    }

    private static List<Scope> SeedScopes(StoreDbContext dbContext, Realm masterRealm)
    {
        var allScopeNames = dbContext.Scopes.Select(s => s.Name);
        var unsupportedScopes = SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.Scopes.Where(s => !allScopeNames.Contains(s.Name));
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
        var confClientIds = SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.Clients.Select(c => c.ClientId);
        var allClientIds = dbContext.Clients.Select(s => s.ClientId);
        var unknownClients = SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.Clients.Where(c => !allClientIds.Contains(c.ClientId));
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
            var cl = SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.Clients.Single(c => c.ClientId == knownClient.ClientId);
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

    private static void SeedGotifySessions(StoreDbContext dbContext)
    {
        if (!dbContext.GotifySessions.Any())
            dbContext.GotifySessions.AddRange(SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.Sessions);
    }

    private static void SeedAuthSchemes(StoreDbContext dbContext)
    {
        foreach (var providerDefinition in SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.ProviderDefinitions)
        {
            if (!dbContext.AuthenticationSchemeProviderDefinitions.Any(d => d.Name == providerDefinition.Name))
            {
                dbContext.AuthenticationSchemeProviderDefinitions.Add(providerDefinition);
            }
        }

        if (!dbContext.AuthenticationSchemeProviders.Any())
        {
            foreach (var provider in SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.Providers)
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
            dbContext.IdentityProvisioningDefinitions.AddRange(SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.IdentityProvisioningDefLst);

        if (!dbContext.IdentityProvisioningLst.Any())
            dbContext.IdentityProvisioningLst.AddRange(SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.GetIdentityProvisiongLst(scimBaseUrl));
    }

    private static void SeedSerializedFileKeys(StoreDbContext dbContext)
    {
        if (!dbContext.SerializedFileKeys.Any())
        {
            dbContext.SerializedFileKeys.AddRange(SimpleIdServer.IdServer.Config.DefaultKeys);
            dbContext.SerializedFileKeys.Add(WsFederationKeyGenerator.GenerateWsFederationSigningCredentials(SimpleIdServer.IdServer.Config.DefaultRealms.Master));
        }
    }

    private static void SeedCertificateAuthorities(StoreDbContext dbContext)
    {
        if (!dbContext.CertificateAuthorities.Any())
            dbContext.CertificateAuthorities.AddRange(SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.CertificateAuthorities);
    }

    private static void SeedPresentationDefinitions(StoreDbContext dbContext)
    {
        if (!dbContext.PresentationDefinitions.Any())
            dbContext.PresentationDefinitions.AddRange(SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.PresentationDefinitions);
    }

    private static void SeedFederationEntities(StoreDbContext dbContext)
    {
        if (!dbContext.FederationEntities.Any())
            dbContext.FederationEntities.AddRange(SimpleIdServer.IdServer.Startup.Conf.IdServerConfiguration.FederationEntities);
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
            if(formDbContext.Forms.Any())
            {
                return;
            }

            var allForms = new List<FormRecord>();
            allForms.Add(FormBuilder.Constants.EmptyStep);
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
