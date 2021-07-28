// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class DeleteRepresentationCommandHandler : IDeleteRepresentationCommandHandler
    {
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly IRepresentationReferenceSync _representationReferenceSync;

        public DeleteRepresentationCommandHandler(ISCIMRepresentationCommandRepository scimRepresentationCommandRepository,
            ISCIMRepresentationQueryRepository scimRepresentationQueryRepository,
            IRepresentationReferenceSync representationReferenceSync)
        {
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _representationReferenceSync = representationReferenceSync;
        }

        public async Task<SCIMRepresentation> Handle(DeleteRepresentationCommand request)
        {
            var representation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(request.Id, request.ResourceType);
            if (representation == null)
            {
                throw new SCIMNotFoundException(string.Format(Global.ResourceNotFound, request.Id));
            }

            var references = await _representationReferenceSync.Sync(request.ResourceType, representation, representation, true);
            using (var transaction = await _scimRepresentationCommandRepository.StartTransaction())
            {
                await _scimRepresentationCommandRepository.Delete(representation);
                foreach (var reference in references)
                {
                    await _scimRepresentationCommandRepository.Update(reference);
                }

                await transaction.Commit();
            }

            return representation;
        }
    }
}