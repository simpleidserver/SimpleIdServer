// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Extractors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Extractors
{
    public interface ICredentialTemplateClaimsExtractor
    {
        Task<Dictionary<string, object>> ExtractClaims(HandlerContext context, CredentialTemplate credentialTemplate);
    }

    public class CredentialTemplateClaimsExtractor : ICredentialTemplateClaimsExtractor
    {
        private readonly IClaimsExtractor _claimsExtractor;

        public CredentialTemplateClaimsExtractor(IClaimsExtractor claimsExtractor)
        {
            _claimsExtractor = claimsExtractor;
        }

        public Task<Dictionary<string, object>> ExtractClaims(HandlerContext context, CredentialTemplate credentialTemplate)
        {
            return _claimsExtractor.ResolveGroupsAndExtract(context, credentialTemplate.ClaimMappers);
        }
    }
}
