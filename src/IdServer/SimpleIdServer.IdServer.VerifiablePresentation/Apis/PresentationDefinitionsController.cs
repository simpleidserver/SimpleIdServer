// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.VerifiablePresentation.Resources;
using System.Net;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis;

public class PresentationDefinitionsController : BaseController
{
    private readonly ILogger<PresentationDefinitionsController> _logger;
    private readonly IPresentationDefinitionStore _presentationDefinitionStore;

    public PresentationDefinitionsController(
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        IPresentationDefinitionStore presentationDefinitionStore,
        ILogger<PresentationDefinitionsController> logger) : base(tokenRepository, jwtBuilder)
    {
        _presentationDefinitionStore = presentationDefinitionStore;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        try
        {
            var presentationDefinition = await _presentationDefinitionStore.GetByPublicId(id, prefix, cancellationToken);
            if(presentationDefinition == null)
                throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownPresentationDefinition, id));
            return new OkObjectResult(presentationDefinition);
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}