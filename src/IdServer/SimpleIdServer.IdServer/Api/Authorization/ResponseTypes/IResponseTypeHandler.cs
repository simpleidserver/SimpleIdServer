// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseTypes
{
    public interface IResponseTypeHandler
    {
        string GrantType { get; }
        string ResponseType { get; }
        int Order { get; }
        Task Enrich(EnrichParameter parameter, HandlerContext context, CancellationToken cancellationToken);
    }

    public class EnrichParameter
    {
        public IEnumerable<string> Scopes { get; set; }
        public IEnumerable<string> Audiences { get; set; }
        public IEnumerable<AuthorizationData> AuthorizationDetails { get; set; }
        public IEnumerable<AuthorizedClaim> Claims { get; set; }
        public string GrantId { get; set; }
    }
}