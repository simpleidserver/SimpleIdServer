using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.DTOs;
using SimpleIdServer.Uma.Exceptions;
using SimpleIdServer.Uma.Extensions;
using SimpleIdServer.Uma.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api
{
    [Route(UMAConstants.EndPoints.ResourcesAPI)]
    public class ResourcesAPIController : Controller
    {
        public const string UserAccessPolicyUri = "user_access_policy_uri";
        private readonly IUMAResourceCommandRepository _umaResourceCommandRepository;
        private readonly IUMAResourceQueryRepository _umaResourceQueryRepository;

        public ResourcesAPIController(IUMAResourceCommandRepository umaResourceCommandRepository, IUMAResourceQueryRepository umaResourceQueryRepository)
        {
            _umaResourceCommandRepository = umaResourceCommandRepository;
            _umaResourceQueryRepository = umaResourceQueryRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _umaResourceQueryRepository.GetAll();
            return new OkObjectResult(result.Select(r => r.Id));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _umaResourceQueryRepository.FindByIdentifier(id);
            if (result == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, "not_found");
            }

            return new OkObjectResult(Serialize(result));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JObject jObj)
        {
            try
            {
                var umaResource = BuildUMAResource(jObj, true);
                _umaResourceCommandRepository.Add(umaResource);
                await _umaResourceCommandRepository.SaveChanges();
                var result = new JObject
                {
                    { UMAResourceNames.Id, umaResource.Id },
                    { UserAccessPolicyUri, Url.Action("Edit", "ResourcesUI", new { id = umaResource.Id }) }
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
                return this.BuildError(HttpStatusCode.BadRequest, "invalid_request", ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] JObject jObj)
        {
            try
            {
                var receivedUmaResource = BuildUMAResource(jObj);
                var actualUmaResource = await _umaResourceQueryRepository.FindByIdentifier(id);
                if (actualUmaResource == null)
                {
                    return this.BuildError(HttpStatusCode.NotFound, "not_found");
                }

                actualUmaResource.IconUri = receivedUmaResource.IconUri;
                actualUmaResource.Names = receivedUmaResource.Names;
                actualUmaResource.Descriptions = receivedUmaResource.Descriptions;
                actualUmaResource.Scopes = receivedUmaResource.Scopes;
                actualUmaResource.Type = receivedUmaResource.Type;
                _umaResourceCommandRepository.Update(actualUmaResource);
                await _umaResourceCommandRepository.SaveChanges();
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
                return this.BuildError(HttpStatusCode.BadRequest, "invalid_request", ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var actualUmaResource = await _umaResourceQueryRepository.FindByIdentifier(id);
            if (actualUmaResource == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, "not_found");
            }

            _umaResourceCommandRepository.Delete(actualUmaResource);
            await _umaResourceCommandRepository.SaveChanges();
            return new NoContentResult();
        }

        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> AddPermissions(string id, [FromBody] JObject jObj)
        {
            try
            {
                var permissions = BuildUMAResourcePermissions(jObj);
                var umaResource = await _umaResourceQueryRepository.FindByIdentifier(id);
                if (umaResource == null)
                {
                    return this.BuildError(HttpStatusCode.NotFound, "not_found");
                }

                umaResource.Permissions = permissions;
                _umaResourceCommandRepository.Update(umaResource);
                await _umaResourceCommandRepository.SaveChanges();
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
                return this.BuildError(HttpStatusCode.BadRequest, "invalid_request", ex.Message);
            }
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissions(string id)
        {
            var umaResource = await _umaResourceQueryRepository.FindByIdentifier(id);
            if (umaResource == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, "not_found");
            }

            return new OkObjectResult(Serialize(umaResource.Permissions));
        }

        [HttpDelete("{id}/permissions")]
        public async Task<IActionResult> DeletePermissions(string id)
        {
            var umaResource = await _umaResourceQueryRepository.FindByIdentifier(id);
            if (umaResource == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, "not_found");
            }

            umaResource.Permissions = new List<UMAResourcePermission>();
            return new NoContentResult();
        }

        public static JObject Serialize(UMAResource umaResource)
        {
            var result = new JObject
            {
                { UMAResourceNames.Id, umaResource.Id },
                { UMAResourceNames.ResourceScopes, new JArray(umaResource.Scopes) },
                { UMAResourceNames.IconUri, umaResource.IconUri }
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
                    { UMAResourcePermissionNames.Subject, permission.Subject },
                    { UMAResourcePermissionNames.Scopes, new JArray(permission.Scopes) }
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
            var result = new UMAResource(id);
            var scopes = jObj.GetUMAScopesFromRequest();
            var descriptions = jObj.GetUMADescriptionFromRequest();
            var iconUri = jObj.GetUMAIconURIFromRequest();
            var names = jObj.GetUMANameFromRequest();
            var type = jObj.GetUMATypeFromRequest();
            if (!scopes.Any())
            {
                throw new UMAInvalidRequestException($"parameter {UMAResourceNames.ResourceScopes} is missing");
            }

            if (isHttpPost)
            {
                var subject = jObj.GetUMASubjectFromRequest();
                if (string.IsNullOrWhiteSpace(subject))
                {
                    throw new UMAInvalidRequestException($"parameter {UMAResourceNames.Subject} is missing");
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
                throw new UMAInvalidRequestException($"parameter {UMAResourcePermissionNames.Permissions} is missing");
            }
            
            foreach(JObject permissionValue in permissionsToken)
            {
                var subjectToken = permissionValue.SelectToken(UMAResourcePermissionNames.Subject);
                var scopesToken = permissionValue.SelectToken(UMAResourcePermissionNames.Scopes);
                if (subjectToken == null)
                {
                    throw new UMAInvalidRequestException($"parameter {UMAResourcePermissionNames.Permissions}.{UMAResourcePermissionNames.Subject} is missing");
                }

                if (scopesToken == null)
                {
                    throw new UMAInvalidRequestException($"parameter {UMAResourcePermissionNames.Permissions}.{UMAResourcePermissionNames.Scopes} is missing");
                }

                result.Add(new UMAResourcePermission(subjectToken.ToString(), scopesToken.Values<string>().ToList()));
            }

            return result;
        }
    }
}
