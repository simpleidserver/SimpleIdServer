// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Grants
{
    public class GrantsController : BaseController
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IGrantRepository _grantRepository;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly ITransactionBuilder _transactionBuilder;

        public GrantsController(
            ITokenRepository tokenRepository,
            IGrantRepository grantRepository, 
            IGrantedTokenHelper grantedTokenHelper,
            ITransactionBuilder transactionBuilder,
            IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
        {
            _tokenRepository = tokenRepository;
            _grantRepository = grantRepository;
            _grantedTokenHelper = grantedTokenHelper;
            _transactionBuilder = transactionBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            try
            {
                var bearerToken = ExtractBearerToken();
                var grant = await _grantRepository.Get(id, cancellationToken);
                if (grant == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_TARGET, string.Format(Global.UnknownGrant, id));
                if (grant.Status == Domains.ConsentStatus.PENDING) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_TARGET, Global.GrantIsNotAccepted);
                var token = await _grantedTokenHelper.GetAccessToken(bearerToken, cancellationToken);
                if (token == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.UnknownAccessToken);
                var scopes = token.Claims.Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                if (!scopes.Contains(Constants.StandardScopes.GrantManagementQuery.Name)) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.InvalidAccessTokenScope);
                var clientId = token.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
                if (grant.ClientId != clientId) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, string.Format(Global.UnauthorizedClientAccessGrant, clientId));
                return new OkObjectResult(grant);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Revoke(string id, CancellationToken cancellationToken)
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    var bearerToken = ExtractBearerToken();
                    var grant = await _grantRepository.Get(id, cancellationToken);
                    if (grant == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_TARGET, string.Format(Global.UnknownAuthMethods, id));
                    var token = await _grantedTokenHelper.GetAccessToken(bearerToken, cancellationToken);
                    if (token == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.UnknownAccessToken);
                    var scopes = token.Claims.Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                    if (!scopes.Contains(Constants.StandardScopes.GrantManagementRevoke.Name)) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.InvalidAccessTokenScope);
                    var clientId = token.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
                    if (grant.ClientId != clientId) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, string.Format(Global.UnauthorizedClientAccessGrant, clientId));
                    _grantRepository.Remove(grant);
                    var tokens = await _tokenRepository.GetByGrantId(id, cancellationToken);
                    foreach (var t in tokens)
                        _tokenRepository.Remove(t);
                    await transaction.Commit(cancellationToken);
                    return new NoContentResult();
                }
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }
    }
}
