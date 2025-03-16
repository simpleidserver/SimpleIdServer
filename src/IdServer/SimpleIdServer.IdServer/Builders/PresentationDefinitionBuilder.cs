// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Builders;

public class PresentationDefinitionBuilder
{
    private readonly PresentationDefinition _presentationDefinition;

    private PresentationDefinitionBuilder(PresentationDefinition presentationDefinition)
    {
        _presentationDefinition = presentationDefinition;
    }

    public static PresentationDefinitionBuilder New(string id, string name, Domains.Realm realm = null)
    {
        var presentationDefinition = new PresentationDefinition
        {
            Id = Guid.NewGuid().ToString(),
            PublicId = id,
            Name = name,
            RealmName = realm?.Name ?? IdServer.Config.DefaultRealms.Master.Name
        };
        return new PresentationDefinitionBuilder(presentationDefinition);
    }

    public PresentationDefinitionBuilder AddLdpVcInputDescriptor(string id, string name, string type)
    {
        _presentationDefinition.InputDescriptors.Add(new PresentationDefinitionInputDescriptor
        {
            Id = Guid.NewGuid().ToString(),
            PublicId = id,
            Name = name,
            Format = new List<PresentationDefinitionFormat>
            {
                new PresentationDefinitionFormat
                {
                    Id = Guid.NewGuid().ToString(),
                    Format = "ldp_vc",
                    ProofType = "Ed25519Signature2020"
                }
            },
            Constraints = new List<PresentationDefinitionInputDescriptorConstraint>
            {
                new PresentationDefinitionInputDescriptorConstraint
                {
                    Id = Guid.NewGuid().ToString(),
                    Filter = "$.type",
                    Path = new List<string>
                    {
                        type
                    }
                }
            }
        });
        return this;
    }

    public PresentationDefinition Build() => _presentationDefinition;
}
