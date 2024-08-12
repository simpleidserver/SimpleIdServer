// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Console;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Key;
using SimpleIdServer.WalletClient;
using System.Reflection.Emit;
using System.Text.Json;
using System.Web;

// var publicKey = GenerateDidKey();
var publicKey = "did:key:z2dmzD81cgPx8Vki7JbuuMmFYrWPgYoytykUZ3eyqht1j9Kbrz1hh3CRY4pejtRCZ81CZj8PERW5ZtX2MxSEevzFx8knNYgxZm4emyBxZ8pyB2FipG8ciRVnWQeuRu7oJqfpkoQjqhsAzw5h8u5RPG6XEZSarcafMQumUrGtJc3twQRc9J";
var serializedPrivateKey = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "privatekey.json"));
var privateKey = SignatureKeySerializer.Deserialize(serializedPrivateKey);

var tt = DidKeyResolver.New(null).Resolve(publicKey, CancellationToken.None).Result;

var intentData = "openid-credential-offer://?credential_offer_uri=https%3A%2F%2Fapi-conformance.ebsi.eu%2Fconformance%2Fv3%2Fissuer-mock%2Foffers%2Fd62de43c-d06c-481a-87d2-43ff4981b722";
var uri = Uri.TryCreate(intentData, UriKind.Absolute, out Uri r);
var q = r.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);

var resolver = VerifiableCredentialOfferResolverFactory.Build();
var vc = resolver.ResolveByUrl(HttpUtility.UrlDecode(q["credential_offer_uri"]), publicKey, privateKey, CancellationToken.None).Result;
string ss = "";
// EsbiWallet.RegisterEsbiWalletForConformance().Wait();

string GenerateDidKey()
{
    var exportResult = DidKeyGenerator.New().GenerateRandomES256Key().Export(false, true);
    var export = SignatureKeySerializer.Serialize(exportResult.Key);
    File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "privatekey.json"), JsonSerializer.Serialize(export));
    return exportResult.Did;
}