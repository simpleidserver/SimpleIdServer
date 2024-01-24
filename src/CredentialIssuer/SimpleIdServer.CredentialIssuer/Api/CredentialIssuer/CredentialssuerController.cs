// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Store;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialIssuer
{
    public class CredentialIssuerController : Controller
    {
        private readonly ICredentialTemplateStore _credentialTemplateStore;
        private readonly ICredentialDefinitionSerializer _serializer;

        public CredentialIssuerController(
            ICredentialTemplateStore credentialTemplateStore,
            ICredentialDefinitionSerializer serializer)
        {
            _credentialTemplateStore = credentialTemplateStore;
            _serializer = serializer;
        }

        [HttpGet(Constants.EndPoints.CredentialIssuer)]
        public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            var credentialTemplates = await _credentialTemplateStore.GetAll(cancellationToken);
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var result = new CredentialIssuerResult
            {
                CredentialIssuer = issuer,
                CredentialEndpoint = $"{issuer}/{Constants.EndPoints.Credential}",
                CredentialsSupported = credentialTemplates.ToDictionary(kvp => kvp.Id, kvp => _serializer.Serialize(kvp))
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