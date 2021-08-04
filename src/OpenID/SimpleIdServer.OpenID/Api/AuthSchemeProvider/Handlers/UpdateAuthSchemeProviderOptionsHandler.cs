// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Persistence;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.AuthSchemeProvider.Handlers
{
    public interface IUpdateAuthSchemeProviderOptionsHandler
    {
        Task Handle(string id, JObject jObj, CancellationToken cancellationToken);
    }

    public class UpdateAuthSchemeProviderOptionsHandler : IUpdateAuthSchemeProviderOptionsHandler
    {
        private readonly IAuthenticationSchemeProviderRepository _authenticationSchemeProviderRepository;
        private readonly ILogger<UpdateAuthSchemeProviderOptionsHandler> _logger;

        public UpdateAuthSchemeProviderOptionsHandler(
            IAuthenticationSchemeProviderRepository authenticationSchemeProviderRepository,
            ILogger<UpdateAuthSchemeProviderOptionsHandler> logger)
        {
            _authenticationSchemeProviderRepository = authenticationSchemeProviderRepository;
            _logger = logger;
        }

        public async Task Handle(string id, JObject jObj, CancellationToken cancellationToken)
        {
            var authSchemeProvider = await _authenticationSchemeProviderRepository.Get(id, cancellationToken);
            if (authSchemeProvider == null)
            {
                _logger.LogError($"the authentication scheme provider '{id}' doesn't exist");
                throw new UnknownAuthSchemeProviderException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_AUTH_SCHEME_PROVIDER, id));
            }

            Check(jObj, authSchemeProvider);
            authSchemeProvider.UpdateOptions(jObj);
            await _authenticationSchemeProviderRepository.Update(authSchemeProvider, cancellationToken);
            await _authenticationSchemeProviderRepository.SaveChanges(cancellationToken);
        }

        protected void Check(JObject jObj, AuthenticationSchemeProvider authSchemeProvider)
        {
            var optionsType = Type.GetType(authSchemeProvider.OptionsFullQualifiedName);
            var properties = optionsType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(s => s.Name);
            var jObjProperties = jObj.Properties().Select(p => p.Name);
            var unknownProperties = jObjProperties.Where(p => !properties.Contains(p));
            if (unknownProperties.Any())
            {
                var message = string.Format(ErrorMessages.UNKNOWN_AUTH_SCHEME_PROVIDER_PROPERTIES, string.Join(",", unknownProperties));
                _logger.LogError(message);
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, message);
            }
        }
    }
}
