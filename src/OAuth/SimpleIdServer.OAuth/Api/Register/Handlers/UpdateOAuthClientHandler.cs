// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Register.Validators;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
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

    public class UpdateOAuthClientHandler : IUpdateOAuthClientHandler
    {
        private readonly IOAuthClientValidator _oauthClientValidator;
        private readonly IOAuthClientQueryRepository _oauthClientQueryRepository;
        private readonly IOAuthClientCommandRepository _oauthClientCommandRepository;
        private readonly ILogger<UpdateOAuthClientHandler> _logger;

        public UpdateOAuthClientHandler(
            IOAuthClientValidator oauthClientValidator,
            IOAuthClientQueryRepository oauthClientQueryRepository,
            IOAuthClientCommandRepository oAuthClientCommandRepository,
            ILogger<UpdateOAuthClientHandler> logger)
        {
            _oauthClientValidator = oauthClientValidator;
            _oauthClientQueryRepository = oauthClientQueryRepository;
            _oauthClientCommandRepository = oAuthClientCommandRepository;
            _logger = logger;
        }

        public async Task<JObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var accessToken = handlerContext.Request.GetToken(AutenticationSchemes.Bearer, AutenticationSchemes.Basic);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogError("access token is missing");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, ErrorMessages.MISSING_ACCESS_TOKEN);
            }

            var clients = await _oauthClientQueryRepository.Find(new Persistence.Parameters.SearchClientParameter
            {
                RegistrationAccessToken = accessToken
            }, cancellationToken);
            if (!clients.Content.Any())
            {
                _logger.LogError($"access token '{accessToken}' is invalid");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, ErrorMessages.BAD_ACCESS_TOKEN);
            }

            var oauthClient = clients.Content.First();
            if (oauthClient.ClientId != clientId)
            {
                oauthClient.RegistrationAccessToken = null;
                await _oauthClientCommandRepository.Update(oauthClient, cancellationToken);
                await _oauthClientCommandRepository.SaveChanges(cancellationToken);
                _logger.LogError($"access token '{accessToken}' can be used for the client '{oauthClient.ClientId}' and not for the client '{clientId}'");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, string.Format(ErrorMessages.ACCESS_TOKEN_VALID_CLIENT, oauthClient.ClientId, clientId));
            }

            var extractedClient = ExtractClient(handlerContext);
            if (extractedClient.ClientId != oauthClient.ClientId)
            {
                _logger.LogError("the client identifier must be identical");
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_IDENTIFIER_MUST_BE_IDENTICAL);
            }

            if (extractedClient.Secrets.Any() && extractedClient.Secrets.First(_ => _.Type == ClientSecretTypes.SharedSecret).Value != oauthClient.Secrets.First(_ => _.Type == ClientSecretTypes.SharedSecret).Value)
            {
                _logger.LogError("the client secret must be identical");
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_SECRET_MUST_BE_IDENTICAL);
            }

            extractedClient.ClientId = clientId;
            extractedClient.Secrets = oauthClient.Secrets;
            extractedClient.RegistrationAccessToken = oauthClient.RegistrationAccessToken;
            extractedClient.UpdateDateTime = DateTime.UtcNow;
            extractedClient.CreateDateTime = oauthClient.CreateDateTime;
            await _oauthClientValidator.Validate(extractedClient, cancellationToken);
            await _oauthClientCommandRepository.Update(extractedClient, cancellationToken);
            await _oauthClientCommandRepository.SaveChanges(cancellationToken);
            _logger.LogInformation($"the client '{clientId}' has been updated");
            return null;
        }

        protected virtual OAuthClient ExtractClient(HandlerContext handlerContext)
        {
            var result = handlerContext.Request.Data.ToDomain();
            return result;
        }
    }
}
