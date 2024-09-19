// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using SimpleIdServer.FastFed.Stores;
using SimpleIdServer.IdServer.IntegrationEvents;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim.Sid;

public class SidIntegrationEventsConsumer : IConsumer<AddUserSuccessEvent>
{
    private readonly IExtractedRepresentationStore _extractedRepresentationStore;

    public SidIntegrationEventsConsumer(IExtractedRepresentationStore extractedRepresentationStore)
    {
        _extractedRepresentationStore = extractedRepresentationStore;
    }

    public async Task Consume(ConsumeContext<AddUserSuccessEvent> context)
    {
        var scimRepresentation = new JsonObject();
        var schemas = new JsonArray();
        schemas.Add("urn:ietf:params:scim:schemas:core:2.0:User");
        scimRepresentation.Add("schemas", schemas);
        scimRepresentation.Add("externalId", context.Message.Id);
        scimRepresentation.Add("userName", context.Message.Name);
        var name = new JsonObject();
        if (!string.IsNullOrWhiteSpace(context.Message.Lastname))
            name.Add("familyName", context.Message.Lastname);
        if (!string.IsNullOrWhiteSpace(context.Message.Firstname))
            name.Add("givenName", context.Message.Firstname);

        scimRepresentation.Add("name", name);
        _extractedRepresentationStore.Add(new Models.ExtractedRepresentation
        {
            Id = Guid.NewGuid().ToString(),
            CreateDateTime = DateTime.UtcNow,
            SerializedRepresentation = scimRepresentation.ToJsonString(),
            ProvisioningProfileName = FastFed.Provisioning.Scim.Constants.ProvisioningProfileName
        });
        await _extractedRepresentationStore.SaveChanges(context.CancellationToken);
    }
}
