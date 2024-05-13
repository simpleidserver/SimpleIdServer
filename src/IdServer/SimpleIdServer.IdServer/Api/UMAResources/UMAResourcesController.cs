// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.UMAResources
{
    public class UMAResourcesController : BaseController
    {
        public const string UserAccessPolicyUri = "user_access_policy_uri";
        private readonly IUmaResourceRepository _umaResourceRepository;
        private readonly ILogger<UMAResourcesController> _logger;

        public UMAResourcesController(
            IUmaResourceRepository umaResourceRepository, 
            ITokenRepository tokenRepository, 
            IJwtBuilder jwtBuilder, 
            ILogger<UMAResourcesController> logger) : base(tokenRepository, jwtBuilder)
        {
            _umaResourceRepository = umaResourceRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckHasPAT(prefix);
                var result = await _umaResourceRepository.GetAll(cancellationToken);
                return new OkObjectResult(result.Select(r => r.Id));
            }
            catch(OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOne([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckHasPAT(prefix);
                var result = await _umaResourceRepository.Get(id, cancellationToken);
                if (result == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownUmaResource);
                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] UMAResourceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckHasPAT(prefix);
                Validate(request);
                if(string.IsNullOrWhiteSpace(request.Subject))
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, UMAResourceNames.Subject));
                var umaResource = new UMAResource(Guid.NewGuid().ToString(), DateTime.UtcNow, prefix)
                {
                    IconUri = request.IconUri,
                    Scopes = request.Scopes,
                    Type = request.Type
                };
                umaResource.UpdateTranslations(request.Translations.Select(t => new Translation { Key = t.Name, Language = t.Language, Value = t.Value }));
                _umaResourceRepository.Add(umaResource);
                await _umaResourceRepository.SaveChanges(cancellationToken);
                _logger.LogInformation("UMA resource {UmaResourceId} has been added", umaResource.Id);
                var result = new JsonObject
                {
                    [UMAResourceNames.Id] = umaResource.Id,
                    [UserAccessPolicyUri] = Url.Action("Edit", "Resources", new { id = umaResource.Id })
                };
                return new ContentResult
                {
                    Content = result.ToJsonString(),
                    StatusCode = (int)HttpStatusCode.Created,
                    ContentType = "application/json"
                };
            }
            catch(OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] UMAResourceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await CheckHasPAT(prefix ?? Constants.DefaultRealm);
                Validate(request);
                var currentUmaResource = await _umaResourceRepository.Get(id, cancellationToken);
                if (currentUmaResource == null)
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownUmaResource);

                currentUmaResource.IconUri = request.IconUri;
                currentUmaResource.Scopes = request.Scopes;
                currentUmaResource.Type = request.Type;
                currentUmaResource.UpdateDateTime = DateTime.UtcNow;
                currentUmaResource.UpdateTranslations(request.Translations.Select(t => new Translation { Key = t.Name, Language = t.Language, Value = t.Value }));
                await _umaResourceRepository.SaveChanges(cancellationToken);
                _logger.LogInformation("UMA resource {UmaResourceId} has been updated", currentUmaResource.Id);
                var result = new JsonObject
                {
                    [UMAResourceNames.Id] = currentUmaResource.Id
                };
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = result.ToJsonString(),
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                await CheckHasPAT(prefix ?? Constants.DefaultRealm);
                var currentUmaResource = await _umaResourceRepository.Get(id, cancellationToken);
                if (currentUmaResource == null)
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownUmaResource);

                _umaResourceRepository.Delete(currentUmaResource);
                await _umaResourceRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> AddPermissions([FromRoute] string prefix, string id, [FromBody] UMAResourcePermissionsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await CheckHasPAT(prefix ?? Constants.DefaultRealm);
                var currentUmaResource = await _umaResourceRepository.Get(id, cancellationToken);
                if (currentUmaResource == null)
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownUmaResource);
                Validate(request);
                var permissions = request.Permissions.Select(p =>
                {
                    return new UMAResourcePermission(Guid.NewGuid().ToString(), DateTime.UtcNow)
                    {
                        Scopes = p.Scopes.ToList(),
                        Claims = p.Claims.Select(c => new UMAResourcePermissionClaim
                        {
                            ClaimType = c.ClaimType,
                            FriendlyName = c.ClaimFriendlyName,
                            Name = c.ClaimName,
                            Value = c.ClaimValue
                        }).ToList()
                    };
                });
                currentUmaResource.Permissions = permissions.ToList();
                currentUmaResource.UpdateDateTime = DateTime.UtcNow;
                await _umaResourceRepository.SaveChanges(cancellationToken);
                var result = new JsonObject
                {
                    [UMAResourceNames.Id] = currentUmaResource.Id
                };
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = result.ToString(),
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                await CheckHasPAT(prefix ?? Constants.DefaultRealm);
                var currentUmaResource = await _umaResourceRepository.Get(id, cancellationToken);
                if (currentUmaResource == null)
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownUmaResource);
                return new OkObjectResult(currentUmaResource.Permissions);
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermissions([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                await CheckHasPAT(prefix ?? Constants.DefaultRealm);
                var currentUmaResource = await _umaResourceRepository.Get(id, cancellationToken);
                if (currentUmaResource == null)
                    return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, Global.UnknownUmaResource);
                currentUmaResource.Permissions.Clear();
                await _umaResourceRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        private void Validate(UMAResourceRequest request)
        {
            if (request.Scopes == null || !request.Scopes.Any())
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, UMAResourceNames.ResourceScopes));
        }

        private void Validate(UMAResourcePermissionsRequest request)
        {
            if(request.Permissions == null || !request.Permissions.Any())
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, UMAResourcePermissionNames.Permissions));

            foreach(var permission in request.Permissions)
            {
                if(permission.Claims == null || !permission.Claims.Any())
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, $"{UMAResourcePermissionNames.Permissions}.{UMAResourcePermissionNames.Claims}"));

                if(permission.Scopes == null || !permission.Scopes.Any())
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, $"{UMAResourcePermissionNames.Permissions}.{UMAResourcePermissionNames.Scopes}"));

                foreach(var claim in permission.Claims)
                {
                    if(string.IsNullOrWhiteSpace(claim.ClaimName))
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, $"{UMAResourcePermissionNames.Permissions}.{UMAResourcePermissionNames.Claims}.{UMAResourcePermissionNames.ClaimName}"));

                    if (string.IsNullOrWhiteSpace(claim.ClaimValue))
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, $"{UMAResourcePermissionNames.Permissions}.{UMAResourcePermissionNames.Claims}.{UMAResourcePermissionNames.ClaimValue}"));
                }
            }
        }
    }
}
