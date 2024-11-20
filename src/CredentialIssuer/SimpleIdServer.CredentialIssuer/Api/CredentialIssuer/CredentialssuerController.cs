﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialIssuer
{
    [Route(Constants.EndPoints.CredentialIssuer)]
    public class CredentialIssuerController : Controller
    {
        private readonly ICredentialConfigurationStore _credentialConfigurationStore;
        private readonly ICredentialConfigurationSerializer _serializer;
        private readonly CredentialIssuerOptions _options;

        public CredentialIssuerController(
            ICredentialConfigurationStore credentialConfigurationStore,
            ICredentialConfigurationSerializer serializer,
            IOptions<CredentialIssuerOptions> options)
        {
            _credentialConfigurationStore = credentialConfigurationStore;
            _serializer = serializer;
            _options = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var credentialTemplates = await _credentialConfigurationStore.GetAll(cancellationToken);
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            object result = null;
            if (_options.Version == CredentialIssuerVersion.ESBI)
                result = GetESBICredentialIssuer(issuer, credentialTemplates);
            else
                result = GetLastCredentialIssuer(issuer, credentialTemplates);
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(result),
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/json"
            };
        }

        private object GetLastCredentialIssuer(string issuer, List<Domains.CredentialConfiguration> credentialTemplates)
        {
            // https://openid.net/specs/openid-4-verifiable-credential-issuance-1_0.html
            var result = new LastCredentialIssuerResult
            {
                AuthorizationServers = new List<string>
                {
                    _options.AuthorizationServer
                },
                CredentialIssuer = issuer,
                CredentialEndpoint = $"{issuer}/{Constants.EndPoints.Credential}",
                DeferredCredentialEndpoint = $"{issuer}/{Constants.EndPoints.DeferredCredential}",
                CredentialConfigurationsSupported = credentialTemplates.ToDictionary(kvp => kvp.ServerId, kvp => _serializer.Serialize(kvp)),
                CredentialIdentifiersSupported = true,
                CredentialResponseEncryptionAlgValuesSupported = Constants.AllEncAlgs,
                CredentialResponseEncryptionEncValuesSupported = Constants.AllEncryptions,
                RequireCredentialResponseEncryption = false
            };
            return result;
        }

        private object GetESBICredentialIssuer(string issuer, List<Domains.CredentialConfiguration> credentialTemplates)
        {
            // https://openid.net/specs/openid-4-verifiable-credential-issuance-1_0-10.html#credential-metadata-object
            var result = new ESBICredentialIssuerResult
            {
                AuthorizationServer = _options.AuthorizationServer,
                CredentialIssuer = issuer,
                CredentialEndpoint = $"{issuer}/{Constants.EndPoints.Credential}",
                DeferredCredentialEndpoint = $"{issuer}/{Constants.EndPoints.DeferredCredential}",
                CredentialsSupported = credentialTemplates.Select(kvp => _serializer.Serialize(kvp)).ToList(),
                CredentialIdentifiersSupported = true,
                CredentialResponseEncryptionAlgValuesSupported = Constants.AllEncAlgs,
                CredentialResponseEncryptionEncValuesSupported = Constants.AllEncryptions,
                RequireCredentialResponseEncryption = false
            };
            return result;
        }
    }
}