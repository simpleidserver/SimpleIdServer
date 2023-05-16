// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Vc.Models;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialIssuer
{
    public class CredentialIssuerController : Controller
    {
        private readonly CredentialIssuerOptions _options;
        private readonly ICredentialTemplateRepository _credentialTemplateRepository;

        public CredentialIssuerController(IOptions<CredentialIssuerOptions> options, ICredentialTemplateRepository credentialTemplateRepository)
        {
            _options = options.Value;
            _credentialTemplateRepository = credentialTemplateRepository;
        }

        public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Realm.Constants.DefaultRealm;
            var issuer = SimpleIdServer.Realm.IssuerHelper.GetIssuer(Request.GetAbsoluteUriWithVirtualPath());
            var credentialTemplates = await _credentialTemplateRepository.Query().Include(c => c.Realms).Include(c => c.Parameters).Include(c => c.DisplayLst).Where(c => c.Realms.Any(r => r.Name == prefix)).ToListAsync(cancellationToken);
            var result = new CredentialIssuerResult
            {
                CredentialIssuer = issuer,
                CredentialEndpoint = $"{issuer}/{Constants.EndPoints.Credential}",
                CredentialsSupported = credentialTemplates.Cast<BaseCredentialTemplate>().ToList(),
                Display = _options.CredentialIssuerDisplays
            };
            if (_options.AuthorizationServer != null) result.AuthorizationServer = _options.AuthorizationServer;
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(result),
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/json"
            };
        }
    }
}