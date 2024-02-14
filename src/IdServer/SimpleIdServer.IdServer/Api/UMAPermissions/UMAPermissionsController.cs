// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.UMAPermissions
{
    public class UMAPermissionsController : BaseController
    {
        private readonly IUmaResourceRepository _umaResourceRepository;
        private readonly IUmaPermissionTicketHelper _umaPermissionTicketHelper;
        private readonly ILogger<UMAPermissionsController> _logger;

        public UMAPermissionsController(
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder, 
            IUmaResourceRepository umaResourceRepository, 
            IUmaPermissionTicketHelper umaPermissionTicketHelper, 
            ILogger<UMAPermissionsController> logger) : base(tokenRepository, jwtBuilder)
        {
            _umaResourceRepository = umaResourceRepository;
            _umaPermissionTicketHelper = umaPermissionTicketHelper;
            _logger = logger;
        }

        [HttpPost]
        public Task<IActionResult> Add([FromRoute] string prefix, [FromBody] UMAPermissionRequest request, CancellationToken cancellationToken) => Bulk(prefix, new List<UMAPermissionRequest> { request }, cancellationToken);


        [HttpPost]
        public async Task<IActionResult> Bulk([FromRoute] string prefix, [FromBody] IEnumerable<UMAPermissionRequest> requestLst, CancellationToken cancellationToken)
        {
            try
            {
                await CheckHasPAT(prefix ?? Constants.DefaultRealm);
                var permissionTicket = BuildPermissionTicket(requestLst);
                await Validate(permissionTicket, cancellationToken);
                await _umaPermissionTicketHelper.SetTicket(permissionTicket, cancellationToken);
                return new ContentResult
                {
                    Content = JsonSerializer.Serialize(new UMAPermissionResult { Ticket = permissionTicket.Id }),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Created
                };
            }
            catch(OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(System.Net.HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        private UMAPermissionTicket BuildPermissionTicket(IEnumerable<UMAPermissionRequest> requestLst)
        {
            var records = new List<UMAPermissionTicketRecord>();
            foreach(var request in requestLst)
            {
                if (string.IsNullOrWhiteSpace(request.ResourceId))
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, UMAPermissionNames.ResourceId));

                if (request.Scopes == null || !request.Scopes.Any())
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, UMAPermissionNames.ResourceScopes));

                records.Add(new UMAPermissionTicketRecord(request.ResourceId, request.Scopes.ToList()));
            }

            return new UMAPermissionTicket(Guid.NewGuid().ToString(), records);
        }

        private async Task Validate(UMAPermissionTicket permissionTicket, CancellationToken cancellationToken)
        {
            var resourceIds = permissionTicket.Records.Select(r => r.ResourceId);
            var umaResources = await _umaResourceRepository.Query().Where(r => resourceIds.Contains(r.Id)).ToListAsync(cancellationToken);
            var unknownResources = resourceIds.Where(rid => !umaResources.Any(r => r.Id == rid));
            if(unknownResources.Any())
                throw new OAuthException(ErrorCodes.INVALID_RESOURCE_ID, Global.InvalidResourceId);

            var unknownScopes = permissionTicket.Records.Where(rec => !rec.Scopes.All(sc => umaResources.First(r => r.Id == rec.ResourceId).Scopes.Contains(sc)));
            if(unknownScopes.Any())
                throw new OAuthException(ErrorCodes.INVALID_SCOPE, Global.InvalidUmaPermissionScope);
        }
    }
}
