// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Vc.Models;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialIssuer;

[AllowAnonymous]
public class CredentialIssuerController : Controller
{
    private readonly CredentialIssuerOptions _options;
    private readonly IdServerHostOptions _idOptions;
    private readonly ICredentialTemplateRepository _credentialTemplateRepository;

    public CredentialIssuerController(
        IOptions<CredentialIssuerOptions> options, 
        IOptions<IdServerHostOptions> idOptions,
        ICredentialTemplateRepository credentialTemplateRepository)
    {
        _options = options.Value;
        _idOptions = idOptions.Value;
        _credentialTemplateRepository = credentialTemplateRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
        var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _idOptions.UseRealm);
        var credentialTemplates = await _credentialTemplateRepository.Query().Include(c => c.Realms).Include(c => c.Parameters).Include(c => c.DisplayLst).Where(c => c.Realms.Any(r => r.Name == prefix)).ToListAsync(cancellationToken);
        var result = new CredentialIssuerResult
        {
            CredentialIssuer = issuer,
            CredentialEndpoint = $"{issuer}/{Constants.EndPoints.Credential}",
            CredentialsSupported = credentialTemplates.Cast<BaseCredentialTemplate>().ToList(),
            Display = _options.CredentialIssuerDisplays
        };
        return new ContentResult
        {
            Content = JsonSerializer.Serialize(result),
            StatusCode = (int)HttpStatusCode.OK,
            ContentType = "application/json"
        };
    }
}