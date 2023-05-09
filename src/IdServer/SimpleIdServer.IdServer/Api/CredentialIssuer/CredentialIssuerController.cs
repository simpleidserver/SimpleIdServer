// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Vc.Models;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.CredentialIssuer
{
    public class CredentialIssuerController : Controller
    {
        private readonly ICredentialTemplateRepository _credentialTemplateRepository;
        private readonly IdServerHostOptions _options;

        public CredentialIssuerController(ICredentialTemplateRepository credentialTemplateRepository, IOptions<IdServerHostOptions> options)
        {
            _credentialTemplateRepository = credentialTemplateRepository;
            _options = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath());
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
}
