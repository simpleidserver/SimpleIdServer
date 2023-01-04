// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.AuthSchemeProvider
{
    [Authorize(Constants.Policies.AuthSchemeProvider)]
    public class AuthSchemeProvidersController : Controller
    {
        private readonly IAuthenticationSchemeProviderRepository _repository;
        private readonly ILogger<AuthSchemeProvidersController> _logger;

        public AuthSchemeProvidersController(IAuthenticationSchemeProviderRepository repository, ILogger<AuthSchemeProvidersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            var result = await _repository.Query().AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (result == null) return NotFound();
            return new OkObjectResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _repository.Query().AsNoTracking().ToListAsync(cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Enable(string id, CancellationToken cancellationToken)
        {
            var result = await _repository.Query().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (result == null) return NotFound();
            result.Enable();
            await _repository.SaveChanges(cancellationToken);
            return new NoContentResult();
        }

        [HttpGet]
        public async Task<IActionResult> Disable(string id, CancellationToken cancellationToken)
        {
            var result = await _repository.Query().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (result == null) return NotFound();
            result.Disable();
            await _repository.SaveChanges(cancellationToken);
            return new NoContentResult();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOptions(string id, JsonObject jsonObj, CancellationToken cancellationToken)
        {
            var result = await _repository.Query().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (result == null) return NotFound();
            var unknownProperties = GetInvalidProperties(result, jsonObj);
            if (unknownProperties != null)
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_AUTH_SCHEME_PROVIDER_PROPERTIES, string.Join(",", unknownProperties)));

            result.UpdateOptions(jsonObj);
            await _repository.SaveChanges(cancellationToken);
            return new NoContentResult();

            IEnumerable<string> GetInvalidProperties(AuthenticationSchemeProvider authSchemeProvider, JsonObject jsonObj)
            {
                var optionsType = Type.GetType(authSchemeProvider.OptionsFullQualifiedName);
                var properties = optionsType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(s => s.Name);
                var jsonProperties = jsonObj.Select(p => p.Key);
                var unknownProperties = jsonProperties.Where(p => !properties.Contains(p));
                if (unknownProperties.Any())
                {
                    _logger.LogError($"Properties {string.Join(",", properties)} are not correct");
                    return unknownProperties;
                }

                return null;
            }
        }
    }
}
