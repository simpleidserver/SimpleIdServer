// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleIdServer.IdServer.Api.AuthenticationMethods
{
    public class AuthenticationMethodsController : BaseController
    {
        private readonly IEnumerable<IAuthenticationMethodService> _authMethods;
        private readonly IConfiguration _configuration;
        private readonly IJwtBuilder _jwtBuilder;

        public AuthenticationMethodsController(IEnumerable<IAuthenticationMethodService> authMethods, IConfiguration configuration, IJwtBuilder jwtBuilder)
        {
            _authMethods = authMethods;
            _configuration = configuration;
            _jwtBuilder = jwtBuilder;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _authMethods.Where(a => a.OptionsType != null).Select(a => new AuthenticationMethodResult
            {
                Id = a.Amr,
                Name = a.Name,
                OptionsName = a.OptionsType.Name,
                Values = null
            });
            return new OkObjectResult(result);
        }

        [HttpPut]
        public IActionResult Update([FromRoute] string prefix, string amr, [FromBody] UpdateAuthMethodConfigurationsRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.AuthenticationMethods.Name, _jwtBuilder);
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
        public IActionResult Get([FromRoute] string prefix, string amr)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.AuthenticationMethods.Name, _jwtBuilder);
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
                    Values = values
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
