// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Tests;

public class SerializeFixture
{
    [Test]
    public void When_Serialize_And_Deserialize_SpaClient_Then_Properties_Are_Correct()
    {
        // ARRANGE
        var client = ClientBuilder.BuildUserAgentClient("oauth", "password", null, "https://oauth.tools/callback/code")
            .AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, SimpleIdServer.IdServer.Constants.StandardScopes.Profile)
            .SetClientName("clientname")
            .Build();
        var json = JsonSerializer.Serialize(client);

        // ACT
        var deserializedClient = JsonSerializer.Deserialize<Client>(json);

        // ASSERT
        Assert.AreEqual(client.ClientId, deserializedClient.ClientId);
        Assert.AreEqual(client.ClientName, deserializedClient.ClientName);
        Assert.IsTrue(deserializedClient.Scopes.Any(s => s.Name == "openid"));
        Assert.IsTrue(deserializedClient.Scopes.Any(s => s.Name == "profile"));
    }

    [Test]
    public void When_Serialize_And_Deserialize_HighlySecuredWebsite_Then_Properties_Are_Correct()
    {
        // ARRANGE
        var newClientBuilder = ClientBuilder.BuildTraditionalWebsiteClient("clientid", "clientsecret", null, "http://localhost")
                    .AddScope(new Scope { Name = "openid" })
                    .SetClientName("clientname");
        newClientBuilder.SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256);
        newClientBuilder.SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256);
        newClientBuilder.SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256);
        var ecdsaSig = ClientKeyGenerator.GenerateECDsaSignatureKey("keyId", SecurityAlgorithms.EcdsaSha256);
        newClientBuilder.AddSigningKey(ecdsaSig, SecurityAlgorithms.EcdsaSha256, SecurityKeyTypes.ECDSA);
        newClientBuilder.UseClientPrivateKeyJwtAuthentication();
        newClientBuilder.UseDPOPProof(false);
        var newClient = newClientBuilder.Build();
        var json = JsonSerializer.Serialize(newClient);

        // ACT
        var deserializedClient = JsonSerializer.Deserialize<Client>(json);

        // ASSERT
        Assert.AreEqual(newClient.ClientId, deserializedClient.ClientId);
        Assert.AreEqual(newClient.AuthorizationSignedResponseAlg, deserializedClient.AuthorizationSignedResponseAlg);
        Assert.AreEqual(newClient.IdTokenSignedResponseAlg, deserializedClient.IdTokenSignedResponseAlg);
        Assert.AreEqual(newClient.RequestObjectSigningAlg, deserializedClient.RequestObjectSigningAlg);
        Assert.AreEqual(newClient.SerializedJsonWebKeys.Count(), deserializedClient.SerializedJsonWebKeys.Count());
        Assert.AreEqual(newClient.TokenEndPointAuthMethod, deserializedClient.TokenEndPointAuthMethod);
        Assert.AreEqual(newClient.DPOPBoundAccessTokens, deserializedClient.DPOPBoundAccessTokens);
        Assert.AreEqual(newClient.IsDPOPNonceRequired, deserializedClient.IsDPOPNonceRequired);
    }

    [Test]
    public void When_Serialize_And_Deserialize_MachineClient_Then_Properties_Are_Correct()
    {
        // ARRANGE
        var newClientBuilder = ClientBuilder.BuildDeviceClient("clientid", "clientsecret", null)
                    .AddScope(new Scope { Name = "openid" })
                    .SetClientName("clientname");
        var newClient = newClientBuilder.Build();
        newClient.TokenExchangeType = TokenExchangeTypes.IMPERSONATION;
        var json = JsonSerializer.Serialize(newClient);

        // ACT
        var deserializedClient = JsonSerializer.Deserialize<Client>(json);

        // ASSERT
        Assert.AreEqual(newClient.ClientId, deserializedClient.ClientId);
        Assert.AreEqual(TokenExchangeTypes.IMPERSONATION, deserializedClient.TokenExchangeType);
    }

    [Test]
    public void When_Serialize_And_Deserialize_SamlClient_Then_Properties_AreCorrect()
    {
        // ARRANGE
        var certificate = KeyGenerator.GenerateSelfSignedCertificate();
        var newClientBuilder = SamlSpClientBuilder.BuildSamlSpClient("clientid", "http://localhost", certificate, null)
            .SetClientName("clientname")
            .SetUseAcsArtifact(true);
        var newClient = newClientBuilder.Build();
        var json = JsonSerializer.Serialize(newClient);

        // ACT
        var deserializedClient = JsonSerializer.Deserialize<Client>(json);

        // ASSERT
        Assert.AreEqual(newClient.ClientId, deserializedClient.ClientId);
        Assert.AreEqual(newClient.SerializedJsonWebKeys.Count(), deserializedClient.SerializedJsonWebKeys.Count());
        Assert.AreEqual(newClient.Parameters.Count(), deserializedClient.Parameters.Count());
    }
}
