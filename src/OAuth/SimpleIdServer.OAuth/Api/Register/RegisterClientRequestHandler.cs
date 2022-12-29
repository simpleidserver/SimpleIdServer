// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Domains;
using SimpleIdServer.Domains.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.Store;
using System.Runtime.InteropServices;
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

        public RegisterClientRequestHandler(IHttpClientFactory httpClientFactory, IClientRepository clientRepository)
        {
            _httpClientFactory = httpClientFactory;
            _clientRepository = clientRepository;
        }

        public async Task Handle(RegisterClientRequest request, CancellationToken cancellationToken)
        {
            await Validate(request, cancellationToken);
            var client = new Client
            {
                ClientId = request.ClientId
            };
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
