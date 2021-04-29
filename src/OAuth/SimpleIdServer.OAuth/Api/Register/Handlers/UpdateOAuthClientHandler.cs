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
using System.Linq;
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
            IOAuthClientQueryRepository oauthClientQueryRepository,
            IOAuthClientCommandRepository oAuthClientCommandRepository,
            ILogger<BaseOAuthClientHandler> logger) : base(oauthClientQueryRepository, oAuthClientCommandRepository, logger)
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

            if (extractedClient.Secrets.Any() && extractedClient.Secrets.First(_ => _.Type == ClientSecretTypes.SharedSecret).Value != oauthClient.Secrets.First(_ => _.Type == ClientSecretTypes.SharedSecret).Value)
            {
                Logger.LogError("the client secret must be identical");
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_SECRET_MUST_BE_IDENTICAL);
            }

            extractedClient.ClientId = clientId;
            extractedClient.Secrets = oauthClient.Secrets;
            extractedClient.RegistrationAccessToken = oauthClient.RegistrationAccessToken;
            extractedClient.UpdateDateTime = DateTime.UtcNow;
            extractedClient.CreateDateTime = oauthClient.CreateDateTime;
            await _oauthClientValidator.Validate(extractedClient, cancellationToken);
            await OAuthClientCommandRepository.Update(extractedClient, cancellationToken);
            await OAuthClientCommandRepository.SaveChanges(cancellationToken);
            Logger.LogInformation($"the client '{clientId}' has been updated");
            return null;
        }

        protected virtual OAuthClient ExtractClient(HandlerContext handlerContext)
        {
            var result = handlerContext.Request.RequestData.ToDomain();
            return result;
        }
    }
}
