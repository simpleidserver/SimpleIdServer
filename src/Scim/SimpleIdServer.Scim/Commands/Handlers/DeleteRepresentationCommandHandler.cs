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
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;

        public DeleteRepresentationCommandHandler(ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            IRepresentationReferenceSync representationReferenceSync,
            IBusControl busControl) : base(busControl)
        {
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _representationReferenceSync = representationReferenceSync;
        }

        public async Task<SCIMRepresentation> Handle(DeleteRepresentationCommand request)
        {
            var representation = await _scimRepresentationCommandRepository.Get(request.Id);
            if (representation == null)
            {
                throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, request.Id));
            }

            var references = await _representationReferenceSync.Sync(request.ResourceType, representation, representation, request.Location, true, true);
            using (var transaction = await _scimRepresentationCommandRepository.StartTransaction())
            {
                await _scimRepresentationCommandRepository.Delete(representation);
                foreach (var reference in references.Representations)
                {
                    await _scimRepresentationCommandRepository.Update(reference);
                }

                await transaction.Commit();
            }

            await Notify(references);
            return representation;
        }
    }
}