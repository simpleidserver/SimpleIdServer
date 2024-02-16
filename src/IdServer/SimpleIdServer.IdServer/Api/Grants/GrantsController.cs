// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Store;
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

        public GrantsController(
            ITokenRepository tokenRepository,
            IGrantRepository grantRepository, 
            IGrantedTokenHelper grantedTokenHelper,
            IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
        {
            _tokenRepository = tokenRepository;
            _grantRepository = grantRepository;
            _grantedTokenHelper = grantedTokenHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            try
            {
                var bearerToken = ExtractBearerToken();
                var grant = await _grantRepository.Query().Include(g => g.Scopes).ThenInclude(s => s.AuthorizedResources).Include(g => g.User).FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
                if (grant == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_TARGET, string.Format(Global.UnknownGrant, id));
                if (grant.Status == Domains.ConsentStatus.PENDING) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_TARGET, Global.GrantIsNotAccepted);
                var token = await _grantedTokenHelper.GetAccessToken(bearerToken, cancellationToken);
                if (token == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.UnknownAccessToken);
                var scopes = token.Claims.Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                if(!scopes.Contains(Constants.StandardScopes.GrantManagementQuery.Name)) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.InvalidAccessTokenScope);
                var clientId = token.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
                if(grant.ClientId != clientId) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, string.Format(Global.UnauthorizedClientAccessGrant, clientId));
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
                var bearerToken = ExtractBearerToken();
                var grant = await _grantRepository.Query().FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
                if (grant == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_TARGET, string.Format(Global.UnknownAuthMethods, id));
                var token = await _grantedTokenHelper.GetAccessToken(bearerToken, cancellationToken);
                if (token == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.UnknownAccessToken);
                var scopes = token.Claims.Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                if (!scopes.Contains(Constants.StandardScopes.GrantManagementRevoke.Name)) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, Global.InvalidAccessTokenScope);
                var clientId = token.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
                if (grant.ClientId != clientId) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, string.Format(Global.UnauthorizedClientAccessGrant, clientId));
                _grantRepository.Remove(grant);
                var tokens = await _tokenRepository.Query().Where(t => t.GrantId == id).ToListAsync(cancellationToken);
                foreach (var t in tokens)
                    _tokenRepository.Remove(t);
                await _tokenRepository.SaveChanges(cancellationToken);
                await _grantRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }
    }
}
