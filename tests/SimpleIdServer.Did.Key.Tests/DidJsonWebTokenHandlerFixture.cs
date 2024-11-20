// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;
using SimpleIdServer.Did.Jwt;

namespace SimpleIdServer.Did.Key.Tests;

public class DidJsonWebTokenHandlerFixture
{
    [Test]
    public async Task When_Check_ESBI_Token_Then_Signature_Is_Correct()
    {
        // ARRANGE
        const string did = "did:key:z2dmzD81cgPx8Vki7JbuuMmFYrWPgYoytykUZ3eyqht1j9KbsEYvdrjxMjQ4tpnje9BDBTzuNDP3knn6qLZErzd4bJ5go2CChoPjd5GAH3zpFJP5fuwSk66U5Pq6EhF4nKnHzDnznEP8fX99nZGgwbAh1o7Gj1X52Tdhf7U4KTk66xsA5r#z2dmzD81cgPx8Vki7JbuuMmFYrWPgYoytykUZ3eyqht1j9KbsEYvdrjxMjQ4tpnje9BDBTzuNDP3knn6qLZErzd4bJ5go2CChoPjd5GAH3zpFJP5fuwSk66U5Pq6EhF4nKnHzDnznEP8fX99nZGgwbAh1o7Gj1X52Tdhf7U4KTk66xsA5r";
        // const string did = "did:key:zBMKwGTaNkKdykiviugjbL54PiJbmdw6fwcrzSxbonKH2QsXL79EFtX3zosQWwME8UCGYsmy5eQFKCEdYY4KgHcooXZNxqL8fU7Q4x6EQPj8uPMHPp7D2om1P1FP3WZQKQzRGFt2MUxKEP6yEr9tRf46TJRhAaqA26rChMSDWwE7mATdWYbHfT3eMXVyv2Wh9xPpjzuZvdaRPS5hBfWs8tCPZo2in16Bzpru2D7d8DDMfv4";
        const string idToken = "eyJ0eXAiOiJvcGVuaWQ0dmNpLXByb29mK2p3dCIsImFsZyI6IkVTMjU2Iiwia2lkIjoiZGlkOmtleTp6MmRtekQ4MWNnUHg4VmtpN0pidXVNbUZZcldQZ1lveXR5a1VaM2V5cWh0MWo5S2JzRVl2ZHJqeE1qUTR0cG5qZTlCREJUenVORFAza25uNnFMWkVyemQ0Yko1Z28yQ0Nob1BqZDVHQUgzenBGSlA1ZnV3U2s2NlU1UHE2RWhGNG5Lbkh6RG56bkVQOGZYOTluWkdnd2JBaDFvN0dqMVg1MlRkaGY3VTRLVGs2NnhzQTVyI3oyZG16RDgxY2dQeDhWa2k3SmJ1dU1tRllyV1BnWW95dHlrVVozZXlxaHQxajlLYnNFWXZkcmp4TWpRNHRwbmplOUJEQlR6dU5EUDNrbm42cUxaRXJ6ZDRiSjVnbzJDQ2hvUGpkNUdBSDN6cEZKUDVmdXdTazY2VTVQcTZFaEY0bktuSHpEbnpuRVA4Zlg5OW5aR2d3YkFoMW83R2oxWDUyVGRoZjdVNEtUazY2eHNBNXIifQ.eyJpc3MiOiJkaWQ6a2V5OnoyZG16RDgxY2dQeDhWa2k3SmJ1dU1tRllyV1BnWW95dHlrVVozZXlxaHQxajlLYnNFWXZkcmp4TWpRNHRwbmplOUJEQlR6dU5EUDNrbm42cUxaRXJ6ZDRiSjVnbzJDQ2hvUGpkNUdBSDN6cEZKUDVmdXdTazY2VTVQcTZFaEY0bktuSHpEbnpuRVA4Zlg5OW5aR2d3YkFoMW83R2oxWDUyVGRoZjdVNEtUazY2eHNBNXIiLCJhdWQiOiJodHRwczovL215LWlzc3Vlci5yb2Nrcy9hdXRoIiwiaWF0IjoxNTg5Njk5NTYyLCJleHAiOjE1ODk2OTk5NjIsIm5vbmNlIjoiUEFQUGYzaDlsZXhUdjNXWUhaeDhhalRlIn0.Oa2Mk135r_bxTrJWFOVRwOXiQrE2Vm3bVilmBCLnQhIdAFJ30qFb4d31yCC9ZNCmgzhYGzp39wqJBt5z3jMJ0Q";
        var resolver = DidKeyResolver.New();
        var didDocument = await resolver.Resolve(did, CancellationToken.None);

        // ACT
        var result = DidJsonWebTokenHandler.New().CheckJwt(idToken, didDocument, did);

        // ASSERT
        Assert.True(result);
    }
}
