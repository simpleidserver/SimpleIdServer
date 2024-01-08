// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.AuthenticationMethods
{
    public class AuthenticationMethodsController : BaseController
    {
        private readonly IEnumerable<IAuthenticationMethodService> _authMethods;
        private readonly IConfiguration _configuration;

        public AuthenticationMethodsController(
            IEnumerable<IAuthenticationMethodService> authMethods, 
            IConfiguration configuration, 
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
        {
            _authMethods = authMethods;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _authMethods.Select(a => new AuthenticationMethodResult
            {
                Id = a.Amr,
                Name = a.Name,
                OptionsName = a.OptionsType?.Name,
                Capabilities = a.Capabilities,
                Values = null
            });
            return new OkObjectResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] string prefix, string amr, [FromBody] UpdateAuthMethodConfigurationsRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.AuthenticationMethods.Name);
                var authMethod = _authMethods.SingleOrDefault(m => m.Amr == amr);
                if (authMethod == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_ACR, string.Format(string.Format(ErrorMessages.AUTHENTICATION_METHOD_NOT_FOUND, amr)));
                if (authMethod.OptionsType == null) return NoContent();
                foreach (var kvp in request.Values)
                    _configuration[$"{authMethod.OptionsType.Name}:{kvp.Key}"] = kvp.Value;
                return NoContent();
            }
            catch(OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string prefix, string amr)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.AuthenticationMethods.Name);
                var authMethod = _authMethods.SingleOrDefault(m => m.Amr == amr);
                if (authMethod == null) return BuildError(System.Net.HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_ACR, string.Format(string.Format(ErrorMessages.AUTHENTICATION_METHOD_NOT_FOUND, amr)));
                if (authMethod.OptionsType == null) return NoContent();
                var section = _configuration.GetSection(authMethod.OptionsType.Name);
                var configuration = section.Get(authMethod.OptionsType);
                var values = new Dictionary<string, string>();
                if (configuration != null)
                {
                    var type = configuration.GetType();
                    var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(configuration)?.ToString();
                        if (!string.IsNullOrWhiteSpace(value)) values.Add(property.Name, value);
                    }
                }

                var result = new AuthenticationMethodResult
                {
                    Id = amr,
                    Name = authMethod.Name,
                    OptionsName = authMethod.OptionsType.Name,
                    Values = values,
                    Capabilities = authMethod.Capabilities
                };
                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }
    }
}
