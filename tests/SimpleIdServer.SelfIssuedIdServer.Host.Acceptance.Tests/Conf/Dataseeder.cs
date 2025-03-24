using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Store.EF;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SimpleIdServer.SelfIdServer.Host.Acceptance.Tests.Conf;

public class Dataseeder
{
    public static void Seed(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using (var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>())
            {
                if(dbContext.Realms.Any())
                {
                    return;
                }

                dbContext.Realms.AddRange(IdServerConfiguration.Realms);
                dbContext.Languages.AddRange(DefaultLanguages.All);
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
