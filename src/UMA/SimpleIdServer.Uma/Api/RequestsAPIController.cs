// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Uma.Domains;
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
    [Route(UMAConstants.EndPoints.RequestsAPI)]
    public class RequestsAPIController : BaseAPIController
    {
        private readonly IUMAPendingRequestRepository _umaPendingRequestRepository;
        private readonly IUMAResourceRepository _umaResourceRepository;
        private readonly IEnumerable<IClaimTokenFormat> _claimTokenFormats;

        public RequestsAPIController(
            IUMAPendingRequestRepository umaPendingRequestRepository,
            IUMAResourceRepository umaResourceRepository, 
            IEnumerable<IClaimTokenFormat> claimTokenFormats,
            IJwtParser jwtParser,
            IOptions<UMAHostOptions> umaHostoptions) : base(jwtParser, umaHostoptions)
        {
            _umaPendingRequestRepository = umaPendingRequestRepository;
            _umaResourceRepository = umaResourceRepository;
            _claimTokenFormats = claimTokenFormats;
        }
               
        [HttpGet(".search/me")]
        public Task<IActionResult> SearchRequests(CancellationToken cancellationToken)
        {
            return CallOperationWithAuthenticatedUser(async (sub, payload) =>
            {
                var searchRequestParameter = ExtractSearchRequestParameter();
                var searchResult = await _umaPendingRequestRepository.FindByRequester(sub, searchRequestParameter, cancellationToken);
                return Serialize(searchRequestParameter, searchResult);
            });
        }

        [HttpGet(".search/received/me")]
        public Task<IActionResult> SearchReceivedRequests(CancellationToken cancellationToken)
        {
            return CallOperationWithAuthenticatedUser(async (sub, payload) =>
            {
                var searchRequestParameter = ExtractSearchRequestParameter();
                var searchResult = await _umaPendingRequestRepository.FindByOwner(sub, searchRequestParameter, cancellationToken);
                return Serialize(searchRequestParameter, searchResult);
            });
        }        

        [HttpGet("confirm/{id}")]
        public Task<IActionResult> Confirm(string id, CancellationToken cancellationToken)
        {
            return CallOperationWithAuthenticatedUser(async (sub, payload) =>
            {
                var pendingRequest = await _umaPendingRequestRepository.FindByTicketIdentifierAndOwner(id, sub, cancellationToken);
                if (pendingRequest == null)
                {
                    return this.BuildError(HttpStatusCode.Unauthorized, UMAErrorCodes.REQUEST_DENIED);
                }

                if (pendingRequest.Status != UMAPendingRequestStatus.TOBECONFIRMED)
                {
                    return this.BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, UMAErrorMessages.REQUEST_CANNOT_BE_CONFIRMED);
                }

                var resource = await _umaResourceRepository.FindByIdentifier(pendingRequest.Resource.Id, cancellationToken);
                foreach(var claimTokenFormat in _claimTokenFormats)
                {
                    resource.Permissions.Add(new UMAResourcePermission(Guid.NewGuid().ToString(), DateTime.UtcNow)
                    {
                        Claims = new List<UMAResourcePermissionClaim>
                        {
                            new UMAResourcePermissionClaim
                            {
                                Name = claimTokenFormat.GetSubjectName(),
                                Value = pendingRequest.Requester
                            }
                        },
                        Scopes = pendingRequest.Scopes.ToList()
                    });
                }

                pendingRequest.Confirm();
                await _umaPendingRequestRepository.Update(pendingRequest, cancellationToken);
                await _umaResourceRepository.Update(resource, cancellationToken);
                await _umaPendingRequestRepository.SaveChanges(cancellationToken);
                await _umaResourceRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            });
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Reject(string id, CancellationToken cancellationToken)
        {
            return CallOperationWithAuthenticatedUser(async (sub, payload) =>
            {
                var pendingRequest = await _umaPendingRequestRepository.FindByTicketIdentifierAndOwner(id, sub, cancellationToken);
                if (pendingRequest == null)
                {
                    return this.BuildError(HttpStatusCode.Unauthorized, UMAErrorCodes.REQUEST_DENIED);
                }

                if (pendingRequest.Status != UMAPendingRequestStatus.TOBECONFIRMED)
                {
                    return this.BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, UMAErrorMessages.REQUEST_CANNOT_BE_CONFIRMED);
                }

                pendingRequest.Reject();
                await _umaPendingRequestRepository.Update(pendingRequest, cancellationToken);
                await _umaPendingRequestRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            });
        }

        public static IActionResult Serialize(SearchRequestParameter searchRequestParameter, SearchResult<UMAPendingRequest> searchResult)
        {
            var result = new JObject
            {
                { "totalResults", searchResult.TotalResults },
                { "count", searchRequestParameter.Count },
                { "startIndex", searchRequestParameter.StartIndex },
                { "data", new JArray(searchResult.Records.Select(rec => Serialize(rec))) }
            };
            return new OkObjectResult(result);
        }

        public static JObject Serialize(UMAPendingRequest umaPendingRequest)
        {
            var result = new JObject
            {
                { "requester", umaPendingRequest.Requester },
                { "owner", umaPendingRequest.Owner },
                { "create_datetime", umaPendingRequest.CreateDateTime },
                { "ticket", umaPendingRequest.TicketId },
                { "scopes", new JArray(umaPendingRequest.Scopes) },
                { "status", umaPendingRequest.Status.ToString().ToLowerInvariant() },
                { "resource", ResourcesAPIController.Serialize(umaPendingRequest.Resource)}
            };
            return result;
        }
    }
}
