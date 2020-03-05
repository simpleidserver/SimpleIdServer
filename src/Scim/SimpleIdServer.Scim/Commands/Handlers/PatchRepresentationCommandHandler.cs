// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Commands.Handlers
{
    public class PatchRepresentationCommandHandler : IPatchRepresentationCommandHandler
    {
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly ISCIMRepresentationCommandRepository _scimRepresentationCommandRepository;

        public PatchRepresentationCommandHandler(ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, ISCIMRepresentationCommandRepository scimRepresentationCommandRepository)
        {
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _scimRepresentationCommandRepository = scimRepresentationCommandRepository;
        }

        public async Task<SCIMRepresentation> Handle(PatchRepresentationCommand patchRepresentationCommand)
        {
            var patches = ExtractPatchOperationsFromRequest(patchRepresentationCommand.Content);
            var existingRepresentation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(patchRepresentationCommand.Id);
            if (existingRepresentation == null)
            {
                throw new SCIMNotFoundException($"Resource '{patchRepresentationCommand.Id}' does not exist");
            }

            existingRepresentation.ApplyPatches(patches);
            existingRepresentation.SetUpdated(DateTime.UtcNow);
            _scimRepresentationCommandRepository.Update(existingRepresentation);
            await _scimRepresentationCommandRepository.SaveChanges();
            return existingRepresentation;
        }

        private ICollection<SCIMPatchOperationRequest> ExtractPatchOperationsFromRequest(JObject content)
        {
            var requestedSchemas = content.GetSchemas();
            if (!requestedSchemas.Any())
            {
                throw new SCIMBadSyntaxException($"{SCIMConstants.StandardSCIMRepresentationAttributes.Schemas} attribute is missing");
            }

            if (!requestedSchemas.SequenceEqual(new List<string> { SCIMConstants.StandardSchemas.PatchRequestSchemas.Id }))
            {
                throw new SCIMBadSyntaxException("some schemas are not recognized by the endpoint");
            }

            var operationsToken = content.SelectToken("Operations") as JArray;
            if (operationsToken == null)
            {
                throw new SCIMBadSyntaxException("The Operations parameter is missing");
            }

            var result = new List<SCIMPatchOperationRequest>();
            foreach(JObject record in operationsToken)
            {
                SCIMPatchOperations op;
                string path;
                if (record.TryGetEnum<SCIMPatchOperations>("op", out op) && record.TryGetString("path", out path))
                {
                    result.Add(new SCIMPatchOperationRequest(op, path, record.SelectToken("value")));
                }
            }

            return result;
        }
    }
}
