// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.AuthenticationClassReferences
{
    public class AuthenticationClassReferencesController : BaseController
    {
        private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
        private readonly IRealmRepository _realmRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEnumerable<IAuthenticationMethodService> _authMethodServices;
        private readonly ILogger<AuthenticationClassReferencesController> _logger;

        public AuthenticationClassReferencesController(IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, IRealmRepository realmRepository, IJwtBuilder jwtBuilder, IEnumerable<IAuthenticationMethodService> authMethodServices, ILogger<AuthenticationClassReferencesController> logger)
        {
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
            _realmRepository = realmRepository;
            _jwtBuilder = jwtBuilder;
            _authMethodServices = authMethodServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                CheckAccessToken(prefix, Constants.StandardScopes.Acrs.Name, _jwtBuilder);
                var result = await _authenticationContextClassReferenceRepository.Query().Include(a => a.Realms).Where(a => a.Realms.Any(r => r.Name == prefix)).AsNoTracking().OrderBy(a => a.Name).ToListAsync(cancellationToken);
                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddAuthenticationClassReferenceRequest request, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add Authentication Class Reference"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    CheckAccessToken(prefix, Constants.StandardScopes.Acrs.Name, _jwtBuilder);
                    await Validate();
                    var realm = await _realmRepository.Query().SingleAsync(r => r.Name == prefix, cancellationToken);
                    var record = new AuthenticationContextClassReference
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = request.Name,
                        AuthenticationMethodReferences = request.AuthenticationMethodReferences,
                        DisplayName = request.DisplayName,
                        CreateDateTime = DateTime.UtcNow,
                        UpdateDateTime = DateTime.UtcNow
                    };
                    record.Realms.Add(realm);
                    _authenticationContextClassReferenceRepository.Add(record);
                    await _authenticationContextClassReferenceRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Authentication Class Reference has been added");
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(record),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Created
                    };
                }
                catch(OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    _logger.LogError(ex.ToString());
                    return BuildError(ex);
                }
            }

            async Task Validate()
            {
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthenticationContextClassReferenceNames.Name));
                if (request.AuthenticationMethodReferences == null || !request.AuthenticationMethodReferences.Any()) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthenticationContextClassReferenceNames.AuthenticationMethodReferences));
                var supportedAmrs = _authMethodServices.Select(a => a.Amr);
                var unsupportedAmrs = request.AuthenticationMethodReferences.Where(a => !supportedAmrs.Contains(a));
                if (unsupportedAmrs.Any()) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_AMRS, string.Join(",", unsupportedAmrs)));
                var existingAcr = await _authenticationContextClassReferenceRepository.Query().Include(a => a.Realms).AsNoTracking().SingleOrDefaultAsync(a => a.Realms.Any(r => r.Name == prefix) && a.Name == request.Name, cancellationToken);
                if (existingAcr != null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.ACR_WITH_SAME_NAME_EXISTS);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove Authentication Class Reference"))
            {
                prefix = prefix ?? Constants.DefaultRealm;
                CheckAccessToken(prefix, Constants.StandardScopes.Acrs.Name, _jwtBuilder);
                var acr = await _authenticationContextClassReferenceRepository.Query().Include(a => a.Realms).SingleOrDefaultAsync(a => a.Realms.Any(r => r.Name == prefix) && a.Id == id, cancellationToken);
                if (acr == null)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, "Authentication Class Reference doesn't exit");
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_ACR, string.Format(ErrorMessages.UNKNOWN_ACR, id));
                }

                _authenticationContextClassReferenceRepository.Delete(acr);
                await _authenticationContextClassReferenceRepository.SaveChanges(cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok, "Authentication Class Reference has been removed");
                return new NoContentResult();
            }
        }
    }
}
