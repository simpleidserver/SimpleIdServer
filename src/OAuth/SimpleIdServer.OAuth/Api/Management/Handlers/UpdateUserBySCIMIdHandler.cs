// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IUpdateUserBySCIMIdHandler
    {
        Task<bool> Handle(string scimId, JObject jObj, CancellationToken cancellationToken);
    }

    public class UpdateUserBySCIMIdHandler : IUpdateUserBySCIMIdHandler
    {
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly ILogger<UpdateUserBySCIMIdHandler> _logger;

        public UpdateUserBySCIMIdHandler(
            IOAuthUserRepository oauthUserRepository,
            ILogger<UpdateUserBySCIMIdHandler> logger)
        {
            _oauthUserRepository = oauthUserRepository;
            _logger = logger;
        }

        public virtual async Task<bool> Handle(string scimId, JObject jObj, CancellationToken cancellationToken)
        {
            var user = await _oauthUserRepository.FindOAuthUserByClaim(SimpleIdServer.Jwt.Constants.UserClaims.ScimId, scimId, cancellationToken);
            if (user == null)
            {
                _logger.LogError($"the user '{scimId}' doesn't exist");
                throw new OAuthUserNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_USER, scimId));
            }

            UpdateUser(jObj, user);
            await _oauthUserRepository.Update(user, cancellationToken);
            await _oauthUserRepository.SaveChanges(cancellationToken);
            _logger.LogInformation($"the user '{scimId}' has been updated");
            return true;
        }

        protected virtual void UpdateUser(JObject jObj, OAuthUser user)
        {
            if (jObj.ContainsKey(SimpleIdServer.Jwt.Constants.OAuthClaims.Claims))
            {
                var claims = jObj[SimpleIdServer.Jwt.Constants.OAuthClaims.Claims] as JObject;
                foreach(var kvp in claims)
                {
                    user.UpdateClaim(kvp.Key, kvp.Value.ToString());
                }
                return;
            }
        }
    }
}