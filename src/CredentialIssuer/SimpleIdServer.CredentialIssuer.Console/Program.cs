// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Key;
using SimpleIdServer.WalletClient;
using System.Text.Json;
using System.Web;

// var publicKey = GenerateDidKey();
var publicKey = "did:key:z2dmzD81cgPx8Vki7JbuuMmFYrWPgYoytykUZ3eyqht1j9Kbrz1hh3CRY4pejtRCZ81CZj8PERW5ZtX2MxSEevzFx8knNYgxZm4emyBxZ8pyB2FipG8ciRVnWQeuRu7oJqfpkoQjqhsAzw5h8u5RPG6XEZSarcafMQumUrGtJc3twQRc9J";
var serializedPrivateKey = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "privatekey.json"));
var privateKey = SignatureKeySerializer.Deserialize(serializedPrivateKey);

// PreAuthorisedAndDeferredCredential();
VerifiablePresentationExchange();

void InTimeCredential()
{
    var intentData = "openid-credential-offer://?credential_offer_uri=https%3A%2F%2Fapi-conformance.ebsi.eu%2Fconformance%2Fv3%2Fissuer-mock%2Foffers%2F26961abf-f2f6-4e31-810c-7d5b2b07bb99";
    var uri = Uri.TryCreate(intentData, UriKind.Absolute, out Uri r);
    var q = r.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);
    var resolver = VerifiableCredentialOfferResolverFactory.Build();
    var vc = resolver.Resolve(q, publicKey, privateKey, CancellationToken.None).Result;
    Console.WriteLine(vc.Status);
}

void DeferredCredential()
{
    var intentData = "openid-credential-offer://?credential_offer_uri=https%3A%2F%2Fapi-conformance.ebsi.eu%2Fconformance%2Fv3%2Fissuer-mock%2Foffers%2Faa95004c-7dec-453f-9a95-441f306fb171";
    var uri = Uri.TryCreate(intentData, UriKind.Absolute, out Uri r);
    var q = r.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);
    var resolver = VerifiableCredentialOfferResolverFactory.Build();
    var vc = resolver.Resolve(q, publicKey, privateKey, CancellationToken.None).Result;
    Thread.Sleep(6000);
    vc = vc.Retry(CancellationToken.None).Result;
    Console.WriteLine(vc.Status);
}

void PreAuthorisedInTimeCredential()
{
    const string pin = "6267";
    var intentData = "openid-credential-offer://?credential_offer_uri=https%3A%2F%2Fapi-conformance.ebsi.eu%2Fconformance%2Fv3%2Fissuer-mock%2Foffers%2Fd9a9e596-2e4e-437d-ad8a-0cb54519f2ed";
    var uri = Uri.TryCreate(intentData, UriKind.Absolute, out Uri r);
    var q = r.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);
    var resolver = VerifiableCredentialOfferResolverFactory.Build();
    var vc = resolver.Resolve(q, publicKey, privateKey, pin, CancellationToken.None).Result;
    Console.WriteLine(vc.Status);
}

void PreAuthorisedAndDeferredCredential()
{
    const string pin = "6267";
    var intentData = "openid-credential-offer://?credential_offer_uri=https%3A%2F%2Fapi-conformance.ebsi.eu%2Fconformance%2Fv3%2Fissuer-mock%2Foffers%2F2b9fbcb2-b466-4c59-844f-c5d51a1e64b4";
    var uri = Uri.TryCreate(intentData, UriKind.Absolute, out Uri r);
    var q = r.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);
    var resolver = VerifiableCredentialOfferResolverFactory.Build();
    var vc = resolver.Resolve(q, publicKey, privateKey, pin, CancellationToken.None).Result;
    Thread.Sleep(6000);
    vc = vc.Retry(CancellationToken.None).Result;
    Console.WriteLine(vc.Status);
}

void VerifiablePresentationExchange()
{
    var intentData = "openid-credential-offer://?credential_offer_uri=https%3A%2F%2Fapi-conformance.ebsi.eu%2Fconformance%2Fv3%2Fissuer-mock%2Foffers%2F4e04dea2-0fbb-4b58-9626-783bc8920663";
    var uri = Uri.TryCreate(intentData, UriKind.Absolute, out Uri r);
    var q = r.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);
    var resolver = VerifiableCredentialOfferResolverFactory.Build();
    var vc = resolver.Resolve(q, publicKey, privateKey, CancellationToken.None).Result;

}

string GenerateDidKey()
{
    var exportResult = DidKeyGenerator.New().GenerateRandomES256Key().Export(false, true);
    var export = SignatureKeySerializer.Serialize(exportResult.Key);
    File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "privatekey.json"), JsonSerializer.Serialize(export));
    return exportResult.Did;
}