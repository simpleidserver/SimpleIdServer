// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Register.Validators;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public interface IUpdateOAuthClientHandler
    {
        Task<JObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken token);
    }

    public class UpdateOAuthClientHandler : BaseOAuthClientHandler, IUpdateOAuthClientHandler
    {
        private readonly IOAuthClientValidator _oauthClientValidator;

        public UpdateOAuthClientHandler(
            IOAuthClientValidator oauthClientValidator,
            IOAuthClientRepository oAuthClientRepository,
            ILogger<BaseOAuthClientHandler> logger) : base(oAuthClientRepository, logger)
        {
            _oauthClientValidator = oauthClientValidator;
        }

        public async Task<JObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var oauthClient = await GetClient(clientId, handlerContext, cancellationToken);
            var extractedClient = ExtractClient(handlerContext);
            if (extractedClient.ClientId != oauthClient.ClientId)
            {
                Logger.LogError("the client identifier must be identical");
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_IDENTIFIER_MUST_BE_IDENTICAL);
            }

            extractedClient.ClientId = oauthClient.ClientId;
            extractedClient.SetClientSecret(oauthClient.ClientSecret, oauthClient.ClientSecretExpirationTime);
            extractedClient.RegistrationAccessToken = oauthClient.RegistrationAccessToken;
            extractedClient.UpdateDateTime = DateTime.UtcNow;
            extractedClient.CreateDateTime = oauthClient.CreateDateTime;
            await _oauthClientValidator.Validate(extractedClient, cancellationToken);
            await OAuthClientRepository.Update(extractedClient, cancellationToken);
            await OAuthClientRepository.SaveChanges(cancellationToken);
            Logger.LogInformation($"the client '{clientId}' has been updated");
            return null;
        }

        protected virtual BaseClient ExtractClient(HandlerContext handlerContext)
        {
            var result = handlerContext.Request.RequestData.ToDomain();
            return result;
        }
    }
}
