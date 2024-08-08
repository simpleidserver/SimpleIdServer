// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Console;

var intentData = "openid-credential-offer://?credential_offer_uri=https%3A%2F%2Fapi-conformance.ebsi.eu%2Fconformance%2Fv3%2Fissuer-mock%2Foffers%2F4a0f94a0-22a7-4d7c-8bd8-e46d80ab89cc";
var uri = Uri.TryCreate(intentData, UriKind.Absolute, out Uri r);
var q = r.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);

EsbiWallet.RegisterEsbiWalletForConformance().Wait();