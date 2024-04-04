// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
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
        private readonly IRegistrationWorkflowRepository _registrationWorkflowRepository;
        private readonly IEnumerable<IAuthenticationMethodService> _authMethodServices;
        private readonly ILogger<AuthenticationClassReferencesController> _logger;

        public AuthenticationClassReferencesController(
            IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, 
            IRealmRepository realmRepository,
            IRegistrationWorkflowRepository registrationWorkflowRepository,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder, 
            IEnumerable<IAuthenticationMethodService> authMethodServices, 
            ILogger<AuthenticationClassReferencesController> logger) : base(tokenRepository, jwtBuilder)
        {
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
            _realmRepository = realmRepository;
            _registrationWorkflowRepository = registrationWorkflowRepository;
            _authMethodServices = authMethodServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Constants.StandardScopes.Acrs.Name);
                var result = await _authenticationContextClassReferenceRepository
                    .Query()
                    .Include(a => a.Realms)
                    .Include(a => a.RegistrationWorkflow)
                    .Where(a => a.Realms.Any(r => r.Name == prefix)).AsNoTracking().OrderBy(a => a.Name).ToListAsync(cancellationToken);
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
                    await CheckAccessToken(prefix, Constants.StandardScopes.Acrs.Name);
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
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, AuthenticationContextClassReferenceNames.Name));
                if (request.AuthenticationMethodReferences == null || !request.AuthenticationMethodReferences.Any()) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, AuthenticationContextClassReferenceNames.AuthenticationMethodReferences));
                var supportedAmrs = _authMethodServices.Select(a => a.Amr);
                var unsupportedAmrs = request.AuthenticationMethodReferences.Where(a => !supportedAmrs.Contains(a));
                if (unsupportedAmrs.Any()) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnsupportedAmrs, string.Join(",", unsupportedAmrs)));
                var existingAcr = await _authenticationContextClassReferenceRepository.Query().Include(a => a.Realms).AsNoTracking().SingleOrDefaultAsync(a => a.Realms.Any(r => r.Name == prefix) && a.Name == request.Name, cancellationToken);
                if (existingAcr != null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.AcrWithSameNameExists);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove Authentication Class Reference"))
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Constants.StandardScopes.Acrs.Name);
                var acr = await _authenticationContextClassReferenceRepository.Query().Include(a => a.Realms).SingleOrDefaultAsync(a => a.Realms.Any(r => r.Name == prefix) && a.Id == id, cancellationToken);
                if (acr == null)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, "Authentication Class Reference doesn't exit");
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_ACR, string.Format(Global.UnknownAcr, id));
                }

                _authenticationContextClassReferenceRepository.Delete(acr);
                await _authenticationContextClassReferenceRepository.SaveChanges(cancellationToken);
                activity?.SetStatus(ActivityStatusCode.Ok, "Authentication Class Reference has been removed");
                return new NoContentResult();
            }
        }

        [HttpPut]
        public async Task<IActionResult> AssignRegistrationWorkflow([FromRoute] string prefix, string id, [FromBody] AssignRegistrationWorkflowRequest request,  CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Assign registration workflow to the ACR"))
            {
                prefix = prefix ?? Constants.DefaultRealm;
                try
                {
                    await Validate();
                    var acr = await _authenticationContextClassReferenceRepository.Query()
                        .Include(a => a.Realms)
                        .SingleOrDefaultAsync(a => a.Realms.Any(r => r.Name == prefix) && a.Id == id, cancellationToken);
                    if (acr == null)
                    {
                        activity?.SetStatus(ActivityStatusCode.Error, "Authentication Class Reference doesn't exit");
                        return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_ACR, string.Format(Global.UnknownAcr, id));
                    }

                    acr.RegistrationWorkflowId = request.WorkflowId;
                    acr.UpdateDateTime = DateTime.UtcNow;
                    await _authenticationContextClassReferenceRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Registration worklow is assigned to the ACR");
                    return new NoContentResult();
                }
                catch(OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                    return BuildError(ex);
                }
            }

            async Task Validate()
            {
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                if (string.IsNullOrWhiteSpace(request.WorkflowId)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, AuthenticationContextClassReferenceNames.WorkflowId));
                var registrationWorkflow = await _registrationWorkflowRepository.Query()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(r => r.RealmName == prefix && r.Id == request.WorkflowId, cancellationToken);
                if (registrationWorkflow == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, Global.UnknownRegistrationWorkflow);
            }
        }
    }
}
