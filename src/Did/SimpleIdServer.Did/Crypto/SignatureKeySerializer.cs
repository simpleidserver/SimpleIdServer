// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Serializers;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Did.Crypto;

public static class SignatureKeySerializer
{
    public static string SerializedToJson(IAsymmetricKey key)
        => JsonSerializer.Serialize(Serialize(key));

    public static SerializedKey Serialize(IAsymmetricKey key)
        => new SerializedKey
        {
            CrvOrSize = key.CrvOrSize,
            SerializedJwk = JsonWebKeySerializer.Write(key.GetPrivateJwk())
        };

    public static IAsymmetricKey Deserialize(string json)
        => Deserialize(JsonSerializer.Deserialize<SerializedKey>(json));

    public static IAsymmetricKey Deserialize(SerializedKey key)
    {
        var verificationMethod = MulticodecSerializerFactory.AllVerificationMethods.Single(m => m.CrvOrSize == key.CrvOrSize);
        var jwk = key.Jwk;
        return verificationMethod.Build(jwk, jwk);
    }
}

public class SerializedKey
{
    [JsonPropertyName("crv_or_size")]
    public string CrvOrSize { get; set; }
    [JsonPropertyName("jwk")]
    public string SerializedJwk { get; set; }
    [JsonIgnore]
    internal JsonWebKey Jwk
    {
        get
        {
            if (SerializedJwk == null) return null;
            return new JsonWebKey(SerializedJwk);
        }
    }
}