// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IUpdateUserPasswordHandler
    {
        Task Handle(string id, JObject jObj, CancellationToken cancellationToken);
    }

    public class UpdateUserPasswordHandler : IUpdateUserPasswordHandler
    {
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly ILogger<GetOTPCodeHandler> _logger;

        public UpdateUserPasswordHandler(
            IOAuthUserRepository oauthUserRepository,
            ILogger<GetOTPCodeHandler> logger)
        {
            _oauthUserRepository = oauthUserRepository;
            _logger = logger;
        }

        public async Task Handle(string id, JObject jObj, CancellationToken cancellationToken)
        {
            var user = await _oauthUserRepository.FindOAuthUserByLogin(id, cancellationToken);
            if (user == null)
            {
                _logger.LogError($"the user '{id}' doesn't exist");
                throw new OAuthUserNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_USER, id));
            }

            var parameter = JsonConvert.DeserializeObject<UpdatePasswordParameter>(jObj.ToString());
            user.UpdatePassword(parameter.Password);
            await _oauthUserRepository.Update(user, cancellationToken);
            await _oauthUserRepository.SaveChanges(cancellationToken);
            _logger.LogInformation($"password has been updated for the user '{id}'");
        }

        private class UpdatePasswordParameter
        {
            public string Password { get; set; }
        }
    }
}
