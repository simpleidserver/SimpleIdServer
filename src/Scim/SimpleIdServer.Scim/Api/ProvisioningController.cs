// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMEndpoints.Provisioning)]
    public class ProvisioningController : Controller
    {
        private readonly IBusControl _busControl;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly IProvisioningConfigurationRepository _provisioningConfigurationRepository;
        private readonly SCIMHostOptions _options;
        private readonly ILogger<ProvisioningController> _logger;

        public ProvisioningController(
            IBusControl busControl,
            ISCIMRepresentationQueryRepository scimRepresentationQueryRepository,
            IProvisioningConfigurationRepository provisioningConfigurationRepository,
            IOptions<SCIMHostOptions> options,
            ILogger<ProvisioningController> logger)
        {
            _busControl = busControl;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _provisioningConfigurationRepository = provisioningConfigurationRepository;
            _options = options.Value;
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

            if (SCIMConstants.MappingScimResourceTypeToCommonType.ContainsKey(representation.ResourceType))
            {
                var content = representation.ToResponse(string.Empty, false, mergeExtensionAttributes: _options.MergeExtensionAttributes);
                await _busControl.Publish(new RepresentationAddedEvent(representation.Id, representation.Version, SCIMConstants.MappingScimResourceTypeToCommonType[representation.ResourceType], content, _options.IncludeToken ? Request.GetToken() : string.Empty));
            }

            return new NoContentResult();
        }

        [HttpPost("configurations/.search")]
        public async Task<IActionResult> SearchConfigurations([FromBody] SearchProvisioningConfigurationParameter parameter, CancellationToken cancellationToken)
        {
            var configurations = await _provisioningConfigurationRepository.SearchConfigurations(parameter, cancellationToken);
            return new OkObjectResult(configurations);
        }

        [HttpGet("configurations/{id}")]
        public async Task<IActionResult> GetConfiguration(string id, CancellationToken cancellationToken)
        {
            var configuration = await _provisioningConfigurationRepository.GetQuery(id, cancellationToken);
            if (configuration == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(configuration);
        }

        [HttpPut("configurations/{id}")]
        public async Task<IActionResult> UpdateConfiguration(string id, [FromBody] UpdateProvisioningConfigurationParameter parameter, CancellationToken cancellationToken)
        {
            using (var transaction = await _provisioningConfigurationRepository.StartTransaction(cancellationToken))
            {
                var configuration = await _provisioningConfigurationRepository.Get(id, cancellationToken);
                if (configuration == null)
                {
                    return new NotFoundResult();
                }

                configuration.Update(parameter.Records.Select(r => r.ToDomain()));
                await _provisioningConfigurationRepository.Update(configuration, cancellationToken);
                await transaction.Commit();
                return new NoContentResult();
            }
        }

        [HttpPost("histories/.search")]
        public async Task<IActionResult> SearchHistories([FromBody] SearchProvisioningHistoryParameter parameter, CancellationToken cancellationToken)
        {
            var histories = await _provisioningConfigurationRepository.SearchHistory(parameter, cancellationToken);
            return new OkObjectResult(histories);
        }
    }
}
