// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Configuration;

// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.OpenIdConfiguration;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Federation.Apis.OpenidConfiguration;

public class FederationOpenidConfigurationRequestHandler : OpenidConfigurationRequestHandler
{
    public FederationOpenidConfigurationRequestHandler(
        IEnumerable<ISubjectTypeBuilder> subjectTypeBuilders, 
        IScopeRepository scopeRepository, 
        IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, 
        IOptions<IdServerHostOptions> options, 
        IOAuthConfigurationRequestHandler configurationRequestHandler) : base(subjectTypeBuilders, scopeRepository, authenticationContextClassReferenceRepository, options, configurationRequestHandler)
    {
    }

    public override async Task<JsonObject> Handle(string issuer, string prefix, CancellationToken cancellationToken)
    {
        var result = await base.Handle(issuer, prefix, cancellationToken);
        result.Add("client_registration_types_supported", JsonSerializer.SerializeToNode(new List<string>
        {
            "automatic"
        }));
        return result;
    }
}
