// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Store;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialTemplates;

public class CredentialTemplatesController : BaseController
{
    private readonly ICredentialTemplateRepository _credentialTemplateRepository;
    private readonly ILogger<CredentialTemplatesController> _logger;

    public CredentialTemplatesController(ICredentialTemplateRepository credentialTemplateRepository, ILogger<CredentialTemplatesController> logger)
    {
        _credentialTemplateRepository = credentialTemplateRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request)
    {
        return null;
    }
}
