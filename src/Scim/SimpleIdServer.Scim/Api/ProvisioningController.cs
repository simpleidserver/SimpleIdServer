// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.Provisioning)]
    public class ProvisioningController : Controller
    {
        private readonly IBusControl _busControl;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ILogger<ProvisioningController> _logger;

        public ProvisioningController(
            IBusControl busControl,
            ISCIMRepresentationQueryRepository scimRepresentationQueryRepository,
            ILogger<ProvisioningController> logger)
        {
            _busControl = busControl;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [Authorize("Provison")]
        public async Task<IActionResult> Provision(string id)
        {
            var representation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(id);
            if (representation == null)
            {
                _logger.LogError(string.Format(Global.ResourceNotFound, id));
                return this.BuildError(HttpStatusCode.NotFound, string.Format(Global.ResourceNotFound, id));
            }

            var content = representation.ToResponse(string.Empty, false);
            await _busControl.Publish(new RepresentationUpdatedEvent(representation.Id, representation.VersionNumber, representation.ResourceType, content));
            return new NoContentResult();
        }
    }
}
