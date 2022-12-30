// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.Domains;
using SimpleIdServer.Domains.DTOs;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.Store;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register
{
    public interface IRegisterClientRequestHandler
    {

    }

    public class RegisterClientRequestHandler : IRegisterClientRequestHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IClientRepository _clientRepository;
        private readonly OAuthHostOptions _options;

        public RegisterClientRequestHandler(IHttpClientFactory httpClientFactory, IClientRepository clientRepository, IOptions<OAuthHostOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _clientRepository = clientRepository;
            _options = options.Value;
        }

        public async Task Handle(RegisterClientRequest request, CancellationToken cancellationToken)
        {
            await Validate(request, cancellationToken);
            var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            var client = new Client
            {
                ClientId = request.ClientId,
                RegistrationAccessToken = Guid.NewGuid().ToString(),
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                RefreshTokenExpirationTimeInSeconds = _options.DefaultRefreshTokenExpirationTimeInSeconds,
                TokenExpirationTimeInSeconds = _options.DefaultTokenExpirationTimeInSeconds,
                PreferredTokenProfile = _options.DefaultTokenProfile,
                ClientSecret = Guid.NewGuid().ToString()
            };
            if (!string.IsNullOrWhiteSpace(request.ClientName)) client.AddClientName(language, request.ClientName);
            if (request.GrantTypes == null || !request.GrantTypes.Any()) client.GrantTypes = new[] { AuthorizationCodeHandler.GRANT_TYPE };
            if (string.IsNullOrWhiteSpace(request.Scope))
                client.Scopes = _options.DefaultScopes.Select(s => new ClientScope
                {
                    Name = s
                }).ToList();
            else client.Scopes = request.Scope.ToScopes().Select(s => new ClientScope
            {
                Name = s
            }).ToList();
            // CONTINUE...
        }

        protected virtual async Task Validate(RegisterClientRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ClientId)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, OAuthClientParameters.ClientId));
            var existingClient = await _clientRepository.Query().FirstOrDefaultAsync(c => c.ClientId == request.ClientId, cancellationToken);
            if (existingClient != null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.CLIENT_IDENTIFIER_ALREADY_EXISTS, request.ClientId));
        }
    }
}
