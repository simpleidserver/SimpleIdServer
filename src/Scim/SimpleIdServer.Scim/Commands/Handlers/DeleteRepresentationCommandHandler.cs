﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using MassTransit;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Infrastructure;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class DeleteRepresentationCommandHandler : BaseCommandHandler, IDeleteRepresentationCommandHandler
    {
        private readonly ISCIMSchemaCommandRepository _scimSchemaCommandRepository;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;

        public DeleteRepresentationCommandHandler(ISCIMSchemaCommandRepository scimSchemaCommandRepository,
            ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IBusControl busControl) : base(busControl)
        {
            _scimSchemaCommandRepository = scimSchemaCommandRepository;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
        }

        public virtual async Task<GenericResult<SCIMRepresentation>> Handle(DeleteRepresentationCommand request)
        {
            var schema = await _scimSchemaCommandRepository.FindRootSCIMSchemaByResourceType(request.ResourceType);
            if (schema == null) throw new SCIMSchemaNotFoundException();
            var representation = await _scimRepresentationCommandRepository.Get(request.Id);
            if (representation == null) throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, request.Id));
            var references = _representationReferenceSync.Sync(request.ResourceType, representation, representation, request.Location, schema, true, true).ToList();
            using (var transaction = await _scimRepresentationCommandRepository.StartTransaction().ConfigureAwait(false))
            {
                foreach (var reference in references)
                {
                    await _scimRepresentationCommandRepository.BulkInsert(reference.AddedRepresentationAttributes).ConfigureAwait(false);
                    await _scimRepresentationCommandRepository.BulkDelete(reference.RemovedRepresentationAttributes).ConfigureAwait(false);
                }

                await _scimRepresentationCommandRepository.Delete(representation).ConfigureAwait(false);
                await transaction.Commit().ConfigureAwait(false);
                await NotifyAllReferences(references).ConfigureAwait(false);
            }

            return GenericResult<SCIMRepresentation>.Ok(representation);
        }
    }
}