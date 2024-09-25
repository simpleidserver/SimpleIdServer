using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.FastFed.Models;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

namespace SimpleIdServer.FastFed.Host.Acceptance.Tests;

public static class Constants
{
    private static SigningCredentials _sigCredentials = null;

    public static SigningCredentials SigningCredentials
    {
        get
        {
            if (_sigCredentials != null) return _sigCredentials;
            _sigCredentials = GenerateRSASignatureKey("validKeyId");
            return _sigCredentials;
        }
    }

    public static IdentityProviderFederation[] ProviderFederations
    {
        get
        {
            return new [] {
                new IdentityProviderFederation
                {
                    EntityId = "duplicate",
                    Capabilities = new List<IdentityProviderFederationCapabilities>
                    {
                        new IdentityProviderFederationCapabilities
                        {
                            Id = Guid.NewGuid().ToString(),
                            Status = IdentityProviderStatus.CONFIRMED
                        }
                    }
                },
                new IdentityProviderFederation
                {
                    EntityId = "entityId",
                    JwksUri = "http://localhost/jwks",
                    Capabilities = new List<IdentityProviderFederationCapabilities>
                    {
                        new IdentityProviderFederationCapabilities
                        {
                            Id = Guid.NewGuid().ToString(),
                            Status = IdentityProviderStatus.CONFIRMED,
                            AuthenticationProfiles = new List<string>(),
                            ProvisioningProfiles = new List<string>
                            {
                                "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise"
                            }
                        }
                    }
                },
                new IdentityProviderFederation
                {
                    EntityId = "expiredEntityId",
                    JwksUri = "http://localhost/jwks",
                    Capabilities = new List<IdentityProviderFederationCapabilities>
                    {
                        new IdentityProviderFederationCapabilities
                        {
                            Id = Guid.NewGuid().ToString(),
                            Status = IdentityProviderStatus.WHITELISTED,
                            ExpirationDateTime = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds()
                        }
                    }
                }
            };
        }
    }

    public static SigningCredentials GenerateRSASignatureKey(string keyId, string alg = SecurityAlgorithms.RsaSha256)
    {
        var rsa = RSA.Create();
        var securityKey = new RsaSecurityKey(rsa) { KeyId = keyId };
        return new SigningCredentials(securityKey, alg);
    }
}
