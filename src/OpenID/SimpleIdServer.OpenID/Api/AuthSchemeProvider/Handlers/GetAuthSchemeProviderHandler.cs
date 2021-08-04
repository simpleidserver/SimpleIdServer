// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.AuthSchemeProvider.Handlers
{
    public interface IGetAuthSchemeProviderHandler
    {
        Task<JObject> Handle(string id, CancellationToken cancellationToken);
    }

    public class GetAuthSchemeProviderHandler : IGetAuthSchemeProviderHandler
    {
        private readonly IAuthenticationSchemeProviderRepository _authenticationSchemeProviderRepository;
        private readonly ILogger<EnableAuthSchemeProviderHandler> _logger;

        public GetAuthSchemeProviderHandler(
            IAuthenticationSchemeProviderRepository authenticationSchemeProviderRepository,
            ILogger<EnableAuthSchemeProviderHandler> logger)
        {
            _authenticationSchemeProviderRepository = authenticationSchemeProviderRepository;
            _logger = logger;
        }

        public async Task<JObject> Handle(string id, CancellationToken cancellationToken)
        {
            var result = await _authenticationSchemeProviderRepository.Get(id, cancellationToken);
            if (result == null)
            {
                _logger.LogError($"the authentication scheme provider '{id}' doesn't exist");
                throw new UnknownAuthSchemeProviderException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_AUTH_SCHEME_PROVIDER, id));
            }

            return result.ToDto();
        }
    }
}
