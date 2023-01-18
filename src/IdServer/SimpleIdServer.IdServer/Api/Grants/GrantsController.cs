// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
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

        public GrantsController(ITokenRepository tokenRepository, IGrantRepository grantRepository, IGrantedTokenHelper grantedTokenHelper)
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
                var grant = await _grantRepository.Query().Include(g => g.Scopes).FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
                if (grant == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_TARGET, string.Format(ErrorMessages.UNKNOWN_GRANT, id));
                if (grant.Status == Domains.GrantTypeStatus.REVOKED) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.GRANT_IS_REVOKED);

                var token = await _grantedTokenHelper.GetAccessToken(bearerToken, cancellationToken);
                if (token == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, ErrorMessages.UNKNOWN_ACCESS_TOKEN);
                var scopes = token.Claims.Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                if(!scopes.Contains(Constants.StandardScopes.GrantManagementQuery.Name)) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, ErrorMessages.INVALID_ACCESS_TOKEN_SCOPE);
                var clientId = token.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
                if(grant.ClientId != clientId) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, ErrorMessages.INVALID_ACCESS_TOKEN_CLIENTID);

                return new OkObjectResult(grant.Serialize());
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
                if (grant == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_TARGET, string.Format(ErrorMessages.UNKNOWN_GRANT, id));
                if (grant.Status == Domains.GrantTypeStatus.REVOKED) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.GRANT_IS_ALREADY_REVOKED);

                var token = await _grantedTokenHelper.GetAccessToken(bearerToken, cancellationToken);
                if (token == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, ErrorMessages.UNKNOWN_ACCESS_TOKEN);
                var scopes = token.Claims.Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                if (!scopes.Contains(Constants.StandardScopes.GrantManagementQuery.Name)) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, ErrorMessages.INVALID_ACCESS_TOKEN_SCOPE);
                var clientId = token.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
                if (grant.ClientId != clientId) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, ErrorMessages.INVALID_ACCESS_TOKEN_CLIENTID);

                grant.Revoke();
                var tokens = await _tokenRepository.Query().Where(t => t.GrantId == id).ToListAsync(cancellationToken);
                foreach (var t in tokens)
                    _tokenRepository.Remove(t);
                await _tokenRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }
    }
}
