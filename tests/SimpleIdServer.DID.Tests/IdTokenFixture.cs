// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Key;

namespace SimpleIdServer.DID.Tests;

public class IdTokenFixture
{
    [Test]
    public async Task Test()
    {
        var resolver = new DidFactoryResolver(new List<IDidResolver>
        {
            DidKeyResolver.New(new DidKeyOptions
            {
                PublicKeyFormat = JsonWebKey2020Standard.TYPE
            })
        });
        // support JsonWebKey2020 multicodec
        var verificationMethodEncoding = new VerificationMethodEncoding(VerificationMethodStandardFactory.GetAll(), MulticodecSerializerFactory.Build(), MulticodecSerializerFactory.AllVerificationMethods);
        const string idToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NiIsImtpZCI6ImRpZDprZXk6ejJkbXpEODFjZ1B4OFZraTdKYnV1TW1GWXJXUGdZb3l0eWtVWjNleXFodDFqOUtib2o3ZzlQZlhKeGJiczRLWWVneXI3RUxuRlZucERNemJKSkRETlpqYXZYNmp2dERtQUxNYlhBR1c2N3BkVGdGZWEyRnJHR1NGczhFanhpOTZvRkxHSGNMNFA2YmpMRFBCSkV2UlJIU3JHNExzUG5lNTJmY3p0Mk1XakhMTEpCdmhBQyN6MmRtekQ4MWNnUHg4VmtpN0pidXVNbUZZcldQZ1lveXR5a1VaM2V5cWh0MWo5S2JvajdnOVBmWEp4YmJzNEtZZWd5cjdFTG5GVm5wRE16YkpKREROWmphdlg2anZ0RG1BTE1iWEFHVzY3cGRUZ0ZlYTJGckdHU0ZzOEVqeGk5Nm9GTEdIY0w0UDZiakxEUEJKRXZSUkhTckc0THNQbmU1MmZjenQyTVdqSExMSkJ2aEFDIn0.eyJzdWIiOiJkaWQ6a2V5OnoyZG16RDgxY2dQeDhWa2k3SmJ1dU1tRllyV1BnWW95dHlrVVozZXlxaHQxajlLYm9qN2c5UGZYSnhiYnM0S1llZ3lyN0VMbkZWbnBETXpiSkpERE5aamF2WDZqdnREbUFMTWJYQUdXNjdwZFRnRmVhMkZyR0dTRnM4RWp4aTk2b0ZMR0hjTDRQNmJqTERQQkpFdlJSSFNyRzRMc1BuZTUyZmN6dDJNV2pITExKQnZoQUMiLCJpc3MiOiJkaWQ6a2V5OnoyZG16RDgxY2dQeDhWa2k3SmJ1dU1tRllyV1BnWW95dHlrVVozZXlxaHQxajlLYm9qN2c5UGZYSnhiYnM0S1llZ3lyN0VMbkZWbnBETXpiSkpERE5aamF2WDZqdnREbUFMTWJYQUdXNjdwZFRnRmVhMkZyR0dTRnM4RWp4aTk2b0ZMR0hjTDRQNmJqTERQQkpFdlJSSFNyRzRMc1BuZTUyZmN6dDJNV2pITExKQnZoQUMiLCJhdWQiOiJodHRwczovLzc1ZjktODEtMjQ2LTEzNC0xMTYubmdyb2stZnJlZS5hcHAvbWFzdGVyIiwiaWF0IjoxNzE4MjAwNjA1LCJleHAiOjE3MTgyMDA5MDV9.LWcSQzPvuz6GsHKbcNDM3mzP-1BnFhwO9tqzltQ7l5AdmvJPOLKYDlkJ6-yivCcfkUZ-zrw38xLnAL2q1lkNNw";
        var verifier = new JwtVerifier(resolver, verificationMethodEncoding);
        await verifier.Verify(idToken, CancellationToken.None);
        
    }
}
