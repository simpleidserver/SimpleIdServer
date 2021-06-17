// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
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

        public UpdateUserBySCIMIdHandler(
            IOAuthUserRepository oauthUserRepository)
        {
            _oauthUserRepository = oauthUserRepository;
        }

        public virtual async Task<bool> Handle(string scimId, JObject jObj, CancellationToken cancellationToken)
        {
            var user = await _oauthUserRepository.FindOAuthUserByClaim(SimpleIdServer.Jwt.Constants.UserClaims.ScimId, scimId, cancellationToken);
            if (user == null)
            {
                var newUser = OAuthUser.Create(jObj.GetStr(SimpleIdServer.Jwt.Constants.UserClaims.Subject));
                UpdateUser(jObj, newUser);
                await _oauthUserRepository.Add(user, cancellationToken);
            }
            else
            {
                UpdateUser(jObj, user);
                await _oauthUserRepository.Update(user, cancellationToken);
            }

            await _oauthUserRepository.SaveChanges(cancellationToken);
            return true;
        }

        protected virtual void UpdateUser(JObject jObj, OAuthUser user)
        {
            if (jObj.ContainsKey("claims"))
            {
                var claims = jObj["claims"] as JObject;
                foreach(var kvp in claims)
                {
                    user.UpdateClaim(kvp.Key, kvp.Value.ToString());
                }
                return;
            }
        }
    }
}
