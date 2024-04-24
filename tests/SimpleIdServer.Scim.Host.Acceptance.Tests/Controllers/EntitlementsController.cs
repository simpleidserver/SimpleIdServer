// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Api;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Queries;

namespace SimpleIdServer.Scim.Host.Acceptance.Tests.Controllers
{
    public class EntitlementsController : BaseApiController
    {
        public EntitlementsController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, IReplaceRepresentationCommandHandler replaceRepresentationCommandHandler, IPatchRepresentationCommandHandler patchRepresentationCommandHandler, ISearchRepresentationsQueryHandler searchRepresentationsQueryHandler, IGetRepresentationQueryHandler getRepresentationQueryHandler, IAttributeReferenceEnricher attributeReferenceEnricher, IOptionsMonitor<SCIMHostOptions> options, ILogger<EntitlementsController> logger, IBusControl busControl, IResourceTypeResolver resourceTypeResolver, IUriProvider uriProvider, IRealmRepository realmRepository) : base("Entitlement", addRepresentationCommandHandler, deleteRepresentationCommandHandler, replaceRepresentationCommandHandler, patchRepresentationCommandHandler, searchRepresentationsQueryHandler, getRepresentationQueryHandler, attributeReferenceEnricher, options, logger, busControl, resourceTypeResolver, uriProvider, realmRepository)
        {
        }
    }
}
