// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IGetUserBySCIMIdHandler
    {
        Task<JObject> Handle(string scimId, CancellationToken cancellationToken);
    }

    public class GetUserBySCIMIdHandler : IGetUserBySCIMIdHandler
    {
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly ILogger<GetUserBySCIMIdHandler> _logger;

        public GetUserBySCIMIdHandler(
            IOAuthUserRepository oauthUserRepository,
            ILogger<GetUserBySCIMIdHandler> logger)
        {
            _oauthUserRepository = oauthUserRepository;
            _logger = logger;
        }

        public async Task<JObject> Handle(string scimId, CancellationToken cancellationToken)
        {
            var user = await _oauthUserRepository.FindOAuthUserByClaim(SimpleIdServer.Jwt.Constants.UserClaims.ScimId, scimId, cancellationToken);
            if (user == null)
            {
                _logger.LogError($"the user '{scimId}' doesn't exist");
                throw new OAuthUserNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_USER, scimId));
            }

            return ToDto(user);
        }

        private static JObject ToDto(OAuthUser user)
        {
            var result = new JObject();
            var claims = new JObject();
            foreach(var claim in user.OAuthUserClaims)
            {
                try
                {
                    claims.Add(claim.Name, JObject.Parse(claim.Value));
                }
                catch
                {
                    claims.Add(claim.Name, claim.Value);
                }
            }

            result.Add(OAuthUserParameters.Id, user.Id);
            result.Add(OAuthUserParameters.Status, (int)user.Status);
            result.Add(OAuthUserParameters.CreateDateTime, user.CreateDateTime);
            result.Add(SimpleIdServer.Jwt.Constants.OAuthClaims.Claims, claims);
            return result;
        }
    }
}
