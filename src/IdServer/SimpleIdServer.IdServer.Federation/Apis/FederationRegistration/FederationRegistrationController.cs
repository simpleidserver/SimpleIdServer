// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Federation.Apis.FederationRegistration;

public class FederationRegistrationController : BaseOpenidFederationController
{
    private readonly ILogger<FederationRegistrationController> _logger;
    private readonly IClientRegistrationService _clientRegistrationService;
    private readonly IKeyStore _keyStore;
    private readonly OpenidFederationOptions _options;

    public FederationRegistrationController(ILogger<FederationRegistrationController> logger,
        IClientRegistrationService clientRegistrationService,
        IKeyStore keyStore,
        IOptions<OpenidFederationOptions> options)
    {
        _logger = logger;
        _clientRegistrationService = clientRegistrationService;
        _keyStore = keyStore;
        _options = options.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        using (var reader = new StreamReader(
           Request.Body,
           encoding: Encoding.UTF8,
           detectEncodingFromByteOrderMarks: false
        ))
        {
            var entityStatement = await reader.ReadToEndAsync();
            prefix = prefix ?? Constants.DefaultRealm;
            if (!Request.Headers.ContentType.Contains(OpenidFederationConstants.EntityStatementContentType))
                return Error(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Resources.Global.OnlyEntityStatementIsSupported);
            try
            {
                var signingCredential = GetSigningCredential(prefix);
                var record = await _clientRegistrationService.ExplicitRegisterClient(prefix, entityStatement, cancellationToken);
                var rpEs = record.Item2.EntityStatements.First();
                var taEs = record.Item2.TrustAnchor;
                var openidFederation = new OpenidFederationResult
                {
                    Iat = rpEs.FederationResult.Iat,
                    Exp = rpEs.FederationResult.Exp,
                    Iss = this.GetAbsoluteUriWithVirtualPath(),
                    Sub = rpEs.FederationResult.Sub,
                    Jwks = rpEs.FederationResult.Jwks
                };
                var client = JsonObject.Parse(JsonSerializer.Serialize(record.Item1)).AsObject();
                openidFederation.Metadata.OtherParameters.Add(EntityStatementTypes.OpenidRelyingParty, client);
                openidFederation.TrustAnchorId = taEs.FederationResult.Sub;
                var handler = new JsonWebTokenHandler();
                var jws = handler.CreateToken(JsonSerializer.Serialize(openidFederation), new SigningCredentials(signingCredential.Key, signingCredential.Algorithm));
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = jws,
                    ContentType = OpenidFederationConstants.EntityStatementContentType
                };
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return Error(ex.StatusCode.Value, ex.Code, ex.Message);
            }
        }
    }

    private SigningCredentials GetSigningCredential(string realm)
    {
        var signingKeys = _keyStore.GetAllSigningKeys(realm);
        var signingKey = signingKeys.FirstOrDefault(k => k.Kid == _options.TokenSignedKid);
        return signingKey;
    }
}