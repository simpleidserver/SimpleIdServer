// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Helpers;
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

        public async Task<SCIMRepresentation> Handle(DeleteRepresentationCommand request)
        {
            var schema = await _scimSchemaCommandRepository.FindRootSCIMSchemaByResourceType(request.ResourceType);
            if (schema == null) throw new SCIMSchemaNotFoundException();
            var representation = await _scimRepresentationCommandRepository.Get(request.Id);
            if (representation == null)
            {
                throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, request.Id));
            }

            var references = _representationReferenceSync.Sync(request.ResourceType, representation, representation, request.Location, true, true);
            using (var transaction = await _scimRepresentationCommandRepository.StartTransaction())
            {
                await _scimRepresentationCommandRepository.Delete(representation);
                foreach (var reference in references)
                {
                    await _scimRepresentationCommandRepository.BulkUpdate(reference.AddedRepresentationAttributes);
                    await _scimRepresentationCommandRepository.BulkDelete(reference.RemovedRepresentationAttributes);
                    await Notify(reference);
                }

                await transaction.Commit();
            }

            return representation;
        }
    }
}