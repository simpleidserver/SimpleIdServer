// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.DTOs;
using SimpleIdServer.Uma.Exceptions;
using SimpleIdServer.Uma.Extensions;
using SimpleIdServer.Uma.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api
{
    [Route(UMAConstants.EndPoints.ResourcesAPI)]
    public class ResourcesAPIController : BaseAPIController
    {
        public const string UserAccessPolicyUri = "user_access_policy_uri";
        private readonly IUMAResourceRepository _umaResourceRepository;

        public ResourcesAPIController(
            IUMAResourceRepository umaResourceRepository, 
            IJwtParser jwtParser, 
            IOptions<UMAHostOptions> umaHostoptions) : base(jwtParser, umaHostoptions)
        {
            _umaResourceRepository = umaResourceRepository;
        }

        #region Operations

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            if (!await IsPATAuthorized(cancellationToken))
            {
                return new UnauthorizedResult();
            }

            var result = await _umaResourceRepository.GetAll(cancellationToken);
            return new OkObjectResult(result.Select(r => r.Id));
        }

        [HttpGet(".search/me")]
        public Task<IActionResult> SearchMe(CancellationToken cancellationToken)
        {
            return CallOperationWithAuthenticatedUser((sub, payload) =>
            {
                return InternalSearch(cancellationToken, sub);                
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            if (!await IsPATAuthorized(cancellationToken))
            {
                return new UnauthorizedResult();
            }

            var result = await _umaResourceRepository.FindByIdentifier(id, cancellationToken);
            if (result == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, UMAErrorCodes.NOT_FOUND);
            }

            return new OkObjectResult(Serialize(result));
        }

        [HttpGet("me/{id}")]
        public Task<IActionResult> GetMe(string id, CancellationToken cancellationToken)
        {
            return CallOperationWithAuthenticatedUser(async (sub, payload) =>
            {
                var result = await _umaResourceRepository.FindByIdentifier(id, cancellationToken);
                if (result == null)
                {
                    return this.BuildError(HttpStatusCode.NotFound, UMAErrorCodes.NOT_FOUND);
                }

                return new OkObjectResult(Serialize(result));
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            if (!await IsPATAuthorized(cancellationToken))
            {
                return new UnauthorizedResult();
            }

            try
            {
                var umaResource = BuildUMAResource(jObj, true);
                await _umaResourceRepository.Add(umaResource, cancellationToken);
                await _umaResourceRepository.SaveChanges(cancellationToken);
                var result = new JObject
                {
                    { UMAResourceNames.Id, umaResource.Id },
                    { UserAccessPolicyUri, Url.Action("Edit", "Resources", new { id = umaResource.Id }) }
                };
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = result.ToString(),
                    StatusCode = (int)HttpStatusCode.Created
                };
            }
            catch(UMAInvalidRequestException ex)
            {
                return this.BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            if (!await IsPATAuthorized(cancellationToken))
            {
                return new UnauthorizedResult();
            }

            try
            {
                var receivedUmaResource = BuildUMAResource(jObj);
                var actualUmaResource = await _umaResourceRepository.FindByIdentifier(id, cancellationToken);
                if (actualUmaResource == null)
                {
                    return this.BuildError(HttpStatusCode.NotFound, UMAErrorCodes.NOT_FOUND);
                }

                actualUmaResource.IconUri = receivedUmaResource.IconUri;
                if (receivedUmaResource.Names != null)
                {
                    actualUmaResource.ClearNames();
                    foreach(var name in receivedUmaResource.Names)
                    {
                        actualUmaResource.AddName(name);
                    }
                }

                if (receivedUmaResource.Descriptions != null)
                {
                    actualUmaResource.ClearDescriptions();
                    foreach(var description in receivedUmaResource.Descriptions)
                    {
                        actualUmaResource.AddDescription(description);
                    }
                }

                actualUmaResource.Scopes = receivedUmaResource.Scopes;
                actualUmaResource.Type = receivedUmaResource.Type;
                await _umaResourceRepository.Update(actualUmaResource, cancellationToken);
                await _umaResourceRepository.SaveChanges(cancellationToken);
                var result = new JObject
                {
                    { UMAResourceNames.Id, actualUmaResource.Id }
                };
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = result.ToString(),
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (UMAInvalidRequestException ex)
            {
                return this.BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            if (!await IsPATAuthorized(cancellationToken))
            {
                return new UnauthorizedResult();
            }

            var actualUmaResource = await _umaResourceRepository.FindByIdentifier(id, cancellationToken);
            if (actualUmaResource == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, UMAErrorCodes.NOT_FOUND);
            }

            await _umaResourceRepository.Delete(actualUmaResource, cancellationToken);
            await _umaResourceRepository.SaveChanges(cancellationToken);
            return new NoContentResult();
        }

        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> AddPermissions(string id, [FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            if (!await IsPATAuthorized(cancellationToken))
            {
                return new UnauthorizedResult();
            }

            try
            {
                var permissions = BuildUMAResourcePermissions(jObj);
                var umaResource = await _umaResourceRepository.FindByIdentifier(id, cancellationToken);
                if (umaResource == null)
                {
                    return this.BuildError(HttpStatusCode.NotFound, UMAErrorCodes.NOT_FOUND);
                }

                umaResource.ReplacePermissions(permissions);
                umaResource.CreateDateTime = DateTime.UtcNow;
                await _umaResourceRepository.Update(umaResource, cancellationToken);
                await _umaResourceRepository.SaveChanges(cancellationToken);
                var result = new JObject
                {
                    { UMAResourceNames.Id, umaResource.Id }
                };
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = result.ToString(),
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (UMAInvalidRequestException ex)
            {
                return this.BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ex.Message);
            }
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissions(string id, CancellationToken cancellationToken)
        {
            if (!await IsPATAuthorized(cancellationToken))
            {
                return new UnauthorizedResult();
            }

            var umaResource = await _umaResourceRepository.FindByIdentifier(id, cancellationToken);
            if (umaResource == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, UMAErrorCodes.NOT_FOUND);
            }

            return new OkObjectResult(Serialize(umaResource.Permissions));
        }

        [HttpDelete("{id}/permissions")]
        public async Task<IActionResult> DeletePermissions(string id, CancellationToken cancellationToken)
        {
            if (!await IsPATAuthorized(cancellationToken))
            {
                return new UnauthorizedResult();
            }

            var umaResource = await _umaResourceRepository.FindByIdentifier(id, cancellationToken);
            if (umaResource == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, UMAErrorCodes.NOT_FOUND);
            }

            umaResource.Permissions = new List<UMAResourcePermission>();
            return new NoContentResult();
        }

        #endregion

        #region Private Methods

        private async Task<IActionResult> InternalSearch(CancellationToken cancellationToken, string subject = null)
        {
            var searchUMAResourceParameter = new SearchUMAResourceParameter();
            EnrichSearchRequestParameter(searchUMAResourceParameter);
            searchUMAResourceParameter.Subject = subject;
            var searchResult = await _umaResourceRepository.Find(searchUMAResourceParameter, cancellationToken);
            var result = new JObject
            {
                { "totalResults", searchResult.TotalResults },
                { "count", searchUMAResourceParameter.Count },
                { "startIndex", searchUMAResourceParameter.StartIndex },
                { "data", new JArray(searchResult.Records.Select(rec => Serialize(rec))) }
            };
            return new OkObjectResult(result);
        }

        public static JObject Serialize(UMAResource umaResource)
        {
            var result = new JObject
            {
                { UMAResourceNames.Id, umaResource.Id },
                { UMAResourceNames.ResourceScopes, new JArray(umaResource.Scopes) },
                { UMAResourceNames.IconUri, umaResource.IconUri },
                { UMAResourceNames.CreateDateTime, umaResource.CreateDateTime }
            };

            Enrich(result, UMAResourceNames.Type, umaResource.Type);
            Enrich(result, UMAResourceNames.Description, umaResource.Descriptions);
            Enrich(result, UMAResourceNames.Name, umaResource.Names);
            return result;
        }

        public static JObject Serialize(ICollection<UMAResourcePermission> permissions)
        {
            var result = new JObject();
            var jArr = new JArray();
            foreach(var permission in permissions)
            {
                jArr.Add(new JObject
                {
                    { UMAResourcePermissionNames.Scopes, new JArray(permission.Scopes) },
                    { UMAResourcePermissionNames.Claims, new JArray(permission.Claims.Select(cl => new JObject
                    {
                        { UMAResourcePermissionNames.ClaimFriendlyName, cl.FriendlyName },
                        { UMAResourcePermissionNames.ClaimName, cl.Name },
                        { UMAResourcePermissionNames.ClaimType, cl.ClaimType },
                        { UMAResourcePermissionNames.ClaimValue, cl.Value }
                    })) }
                });
            }

            result.Add(UMAResourcePermissionNames.Permissions, jArr);
            return result;
        }

        private static void Enrich(JObject jObj, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                jObj.Add(name, value);
            }
        }

        private static void Enrich(JObject jObj, string name, ICollection<OAuthTranslation> translations)
        {
            foreach(var translation in translations)
            {
                jObj.Add($"{name}#{translation.Language}", translation.Value);
            }
        }

        private static UMAResource BuildUMAResource(JObject jObj, bool isHttpPost = false)
        {
            var id = Guid.NewGuid().ToString();
            var result = new UMAResource(id, DateTime.UtcNow);
            var scopes = jObj.GetUMAScopesFromRequest();
            var descriptions = jObj.GetUMADescriptionFromRequest();
            var iconUri = jObj.GetUMAIconURIFromRequest();
            var names = jObj.GetUMANameFromRequest();
            var type = jObj.GetUMATypeFromRequest();
            if (!scopes.Any())
            {
                throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, UMAPermissionNames.ResourceScopes));
            }

            if (isHttpPost)
            {
                var subject = jObj.GetUMASubjectFromRequest();
                if (string.IsNullOrWhiteSpace(subject))
                {
                    throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, UMAResourceNames.Subject));
                }

                result.Subject = subject;
            }

            foreach (var kvp in descriptions)
            {
                result.AddDescription(kvp.Key, kvp.Value);
            }

            foreach (var kvp in names)
            {
                result.AddName(kvp.Key, kvp.Value);
            }

            result.Type = type;
            result.IconUri = iconUri;
            result.Scopes = scopes.ToList();
            return result;
        }

        private static ICollection<UMAResourcePermission> BuildUMAResourcePermissions(JObject jObj)
        {
            var result = new List<UMAResourcePermission>();
            var permissionsToken = jObj.SelectToken(UMAResourcePermissionNames.Permissions);
            if (permissionsToken == null)
            {
                throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, UMAResourcePermissionNames.Permissions));
            }
            
            foreach(JObject permissionValue in permissionsToken)
            {
                var claimsToken = permissionValue.SelectToken(UMAResourcePermissionNames.Claims);
                var scopesToken = permissionValue.SelectToken(UMAResourcePermissionNames.Scopes);
                if (claimsToken == null)
                {
                    throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, $"{UMAResourcePermissionNames.Claims}.{UMAResourcePermissionNames.Claims}"));
                }

                if (scopesToken == null)
                {
                    throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, $"{UMAResourcePermissionNames.Permissions}.{UMAResourcePermissionNames.Scopes}"));
                }

                var permissionClaims = new List<UMAResourcePermissionClaim>();
                foreach (var claimToken in claimsToken)
                {
                    var claimNameToken = claimToken.SelectToken(UMAResourcePermissionNames.ClaimName);
                    var claimValue = claimToken.SelectToken(UMAResourcePermissionNames.ClaimValue);
                    var claimTypeToken = claimToken.SelectToken(UMAResourcePermissionNames.ClaimType);
                    var claimFriendlyNameToken = claimToken.SelectToken(UMAResourcePermissionNames.ClaimFriendlyName);
                    if (claimNameToken == null)
                    {
                        throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, $"{UMAResourcePermissionNames.Claims}.{UMAResourcePermissionNames.Claims}.{UMAResourcePermissionNames.ClaimName}"));
                    }

                    if (claimValue == null)
                    {
                        throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, $"{UMAResourcePermissionNames.Claims}.{UMAResourcePermissionNames.Claims}.{UMAResourcePermissionNames.ClaimValue}"));
                    }

                    permissionClaims.Add(new UMAResourcePermissionClaim
                    {
                        Name = claimNameToken.ToString(),
                        Value = claimValue.ToString(),
                        ClaimType = claimTypeToken?.ToString(),
                        FriendlyName = claimFriendlyNameToken?.ToString()
                    });
                }

                result.Add(new UMAResourcePermission(Guid.NewGuid().ToString(), DateTime.UtcNow, scopesToken.Values<string>().ToList())
                {
                    Claims = permissionClaims
                });
            }

            return result;
        }

        #endregion
    }
}
