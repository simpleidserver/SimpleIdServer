// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.IdServer.Domains;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Tests;

public class PresentationDefinitionSerializer
{
    [Test]
    public void When_Serialize_PresentationDefinition_Then_JsonIsCorrect()
    {
        var presentationDefinition = new PresentationDefinition
        {
            InputDescriptors = new List<PresentationDefinitionInputDescriptor>
            { 
                new PresentationDefinitionInputDescriptor
                {
                    Format = new List<PresentationDefinitionFormat>
                    {
                        new PresentationDefinitionFormat
                        {
                            Format = "ldp_vc",
                            ProofType = "Ed25519Signature2018"
                        }
                    },
                    Constraints = new List<PresentationDefinitionInputDescriptorConstraint>
                    {
                        new PresentationDefinitionInputDescriptorConstraint
                        {
                            Path = new List<string> { "$.type" },
                            Filter = "{\"type\": \"string\", \"pattern\": \"IDCardCredential\"}"
                        }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize(presentationDefinition);
    }
}
