// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Api.Token.TokenBuilders
{
    public interface IOpenBankingApiAuthRequestEnricher
    {
        Task Enrich(JwsPayload result, JObject queryParameters, CancellationToken cancellationToken);
    }
}
