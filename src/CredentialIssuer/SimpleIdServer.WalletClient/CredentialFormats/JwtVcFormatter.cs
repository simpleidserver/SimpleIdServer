// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Vc.Models;
using System.Linq;
using System.Text.Json;

namespace SimpleIdServer.WalletClient.CredentialFormats;

public class JwtVcFormatter : ICredentialFormatter
{
    public string Format => "jwt_vc";

    public W3CVerifiableCredential Extract(string content)
    {
        var handler = new JsonWebTokenHandler();
        var jsonWebToken = handler.ReadJsonWebToken(content);
        var vcJson = jsonWebToken.Claims.Single(c => c.Type == "vc").Value;
        return JsonSerializer.Deserialize<W3CVerifiableCredential>(vcJson);
    }

    public DeserializedCredential DeserializeObject(string serializedVc)
    {
        var handler = new JsonWebTokenHandler();
        var jsonWebToken = handler.ReadJsonWebToken(serializedVc);
        var payload = Base64UrlEncoder.Decode(jsonWebToken.EncodedPayload);
        var header = Base64UrlEncoder.Decode(jsonWebToken.EncodedHeader);
        return new DeserializedCredential(serializedVc, JObject.Parse(payload), JObject.Parse(header));
    }
}
