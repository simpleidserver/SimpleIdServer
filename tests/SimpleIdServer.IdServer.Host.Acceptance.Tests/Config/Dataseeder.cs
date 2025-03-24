// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.EF;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Config;

public static class Dataseeder
{
    public static void Seed(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using(var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>())
            {
                if (dbContext.Realms.Any())
                {
                    return;
                }

                var acrs = new List<AuthenticationContextClassReference> { IdServer.Config.DefaultAcrs.FirstLevelAssurance };
                foreach (var acr in acrs)
                {
                    acr.AuthenticationWorkflow = StandardPwdAuthWorkflows.pwdWorkflowId;
                }

                dbContext.Acrs.AddRange(acrs);
                dbContext.Languages.AddRange(DefaultLanguages.All);
                dbContext.BCAuthorizeLst.AddRange(IdServerConfiguration.BCAuthorizeLst);
                dbContext.Realms.AddRange(IdServerConfiguration.Realms);
                dbContext.Scopes.AddRange(IdServerConfiguration.Scopes);
                dbContext.Clients.AddRange(IdServerConfiguration.Clients);
                dbContext.Users.AddRange(IdServerConfiguration.Users);
                dbContext.ApiResources.AddRange(IdServerConfiguration.ApiResources);
                dbContext.UmaResources.AddRange(IdServerConfiguration.UmaResources);
                dbContext.Groups.AddRange(IdServerConfiguration.Groups);
                dbContext.UserSession.AddRange(IdServerConfiguration.Sessions);
                dbContext.DeviceAuthCodes.AddRange(IdServerConfiguration.DeviceAuthCodes);
                dbContext.FederationEntities.AddRange(IdServerConfiguration.FederationEntities);
                dbContext.SerializedFileKeys.AddRange(new List<SigningCredentials>
                {
                    new SigningCredentials(BuildRsaSecurityKey("keyid"), SecurityAlgorithms.RsaSha256),
                    new SigningCredentials(BuildRsaSecurityKey("keyid2"), SecurityAlgorithms.RsaSha384),
                    new SigningCredentials(BuildRsaSecurityKey("keyid3"), SecurityAlgorithms.RsaSha512),
                    new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP256), SecurityAlgorithms.EcdsaSha256),
                    new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP384), SecurityAlgorithms.EcdsaSha384),
                    new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP521), SecurityAlgorithms.EcdsaSha512),
                    new SigningCredentials(BuildSymmetricSecurityKey(256), SecurityAlgorithms.HmacSha256),
                    new SigningCredentials(BuildSymmetricSecurityKey(384), SecurityAlgorithms.HmacSha384),
                    new SigningCredentials(BuildSymmetricSecurityKey(512), SecurityAlgorithms.HmacSha512)
                }.Select(s => InMemoryKeyStore.Convert(s, DefaultRealms.Master)));
                dbContext.SerializedFileKeys.Add(InMemoryKeyStore.Convert(new EncryptingCredentials(BuildRsaSecurityKey("keyid4"), SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256), DefaultRealms.Master));
                dbContext.SaveChanges();
            }

            using(var dbContext = scope.ServiceProvider.GetRequiredService<FormBuilderDbContext>())
            {
                dbContext.Forms.AddRange(FormBuilderConfiguration.allForms);
                dbContext.Workflows.AddRange(FormBuilderConfiguration.allWorkflows);
                dbContext.SaveChanges();
            }
        }
    }

    private static RsaSecurityKey BuildRsaSecurityKey(string keyid) => new RsaSecurityKey(RSA.Create())
    {
        KeyId = keyid
    };

    private static ECDsaSecurityKey BuildECDSaSecurityKey(ECCurve curve) => new ECDsaSecurityKey(ECDsa.Create(curve))
    {
        KeyId = Guid.NewGuid().ToString()
    };

    private static SecurityKey BuildSymmetricSecurityKey(int keySize) => new SymmetricSecurityKey(GetKey(keySize))
    {
        KeyId = Guid.NewGuid().ToString()
    };

    private static byte[] GetKey(int keySize)
    {
        var length = keySize / 8;
        var str = "abcdefghijklmnopqrstuvwxyz";
        var rnd = new Random();
        return Enumerable.Repeat(0, length).Select(_ => rnd.Next(0, str.Length)).Select(_ => Convert.ToByte(str[_])).ToArray();
    }

}
