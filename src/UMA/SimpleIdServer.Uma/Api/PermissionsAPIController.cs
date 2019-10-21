using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.Uma.Domains;
using SimpleIdServer.Uma.DTOs;
using SimpleIdServer.Uma.Exceptions;
using SimpleIdServer.Uma.Extensions;
using SimpleIdServer.Uma.Helpers;
using SimpleIdServer.Uma.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api
{
    [Route(UMAConstants.EndPoints.PermissionsAPI)]
    public class PermissionsAPIController : BaseAPIController
    {
        private readonly IUMAResourceQueryRepository _umaResourceQueryRepository;
        private readonly IUMAPermissionTicketHelper _permissionTicketHelper;

        public PermissionsAPIController(IUMAResourceQueryRepository umaResourceQueryRepository, IUMAPermissionTicketHelper permissionTicketHelper, IJwtParser jwtParser, IOptions<UMAHostOptions> umaHostoptions) : base(jwtParser, umaHostoptions)
        {
            _umaResourceQueryRepository = umaResourceQueryRepository;
            _permissionTicketHelper = permissionTicketHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Build([FromBody] JToken jToken)
        {
            if (!await IsPATAuthorized())
            {
                return new UnauthorizedResult();
            }

            try
            {
                var permissionTicket = BuildPermissionTicket(jToken);
                await Check(permissionTicket);
                await _permissionTicketHelper.SetTicket(permissionTicket);
                var result = new JObject
                {
                    { UMATokenRequestParameters.Ticket, permissionTicket.Id }
                };
                return new ContentResult
                {
                    Content = result.ToString(),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Created
                };
            }
            catch(UMAInvalidRequestException ex)
            {
                return this.BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ex.Message);
            }
            catch(UMAInvalidResourceException)
            {
                return this.BuildError(HttpStatusCode.BadRequest, UMAErrorCodes.INVALID_RESOURCE_ID, UMAErrorMessages.INVALID_RESOURCE_ID);
            }
            catch(UMAInvalidResourceScopeException)
            {
                return this.BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_SCOPE, UMAErrorMessages.INVALID_SCOPE);
            }
        }

        private UMAPermissionTicket BuildPermissionTicket(JToken jToken)
        {
            var jArr = jToken as JArray;
            var jObj = jToken as JObject;
            if (jObj != null)
            {
                jArr = new JArray();
                jArr.Add(jObj);
            }

            var records = new List<UMAPermissionTicketRecord>();
            foreach(JObject record in jArr)
            {
                var resourceId = record.GetResourceId();
                var resourceScopes = record.GetResourceScopes();
                if (string.IsNullOrWhiteSpace(resourceId))
                {
                    throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, UMAPermissionNames.ResourceId));
                }

                if (resourceScopes == null || !resourceScopes.Any())
                {
                    throw new UMAInvalidRequestException(string.Format(UMAErrorMessages.MISSING_PARAMETER, UMAPermissionNames.ResourceScopes));
                }

                records.Add(new UMAPermissionTicketRecord(resourceId, resourceScopes.ToList()));
            }

            return new UMAPermissionTicket(Guid.NewGuid().ToString(), records);
        }

        private async Task Check(UMAPermissionTicket permissionTicket)
        {
            var resourceIds = permissionTicket.Records.Select(r => r.ResourceId);
            var umaResources = await _umaResourceQueryRepository.FindByIdentifiers(resourceIds);
            var unknownResources = resourceIds.Where(rid => !umaResources.Any(r => r.Id == rid));
            if (unknownResources.Any())
            {
                throw new UMAInvalidResourceException();
            }

            var unknownScopes = permissionTicket.Records.Where(rec => !rec.Scopes.All(sc =>  umaResources.First(r => r.Id == rec.ResourceId).Scopes.Contains(sc)));
            if (unknownScopes.Any())
            {
                throw new UMAInvalidResourceScopeException();
            }
        }
    }
}