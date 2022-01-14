// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.Uma.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenIDUma.Full.Startup
{
    public class UmaDefaultConfiguration
    {
        public static List<UMAResource> Resources = new List<UMAResource>
        {
            new UMAResource(Guid.NewGuid().ToString(), DateTime.UtcNow)
            {
                Translations = new List<UMAResourceTranslation>
                {
                    new UMAResourceTranslation
                    {
                        Translation = new OAuthTranslation(Guid.NewGuid().ToString(), "first resource", "en")
                        {
                            Type = "name"
                        }
                    },
                    new UMAResourceTranslation
                    {
                        Translation = new OAuthTranslation(Guid.NewGuid().ToString(), "première ressource", "fr")
                        {
                            Type = "name"
                        }
                    },
                    new UMAResourceTranslation
                    {
                        Translation = new OAuthTranslation(Guid.NewGuid().ToString(), "first resource", "en")
                        {
                            Type = "description"
                        }
                    },
                    new UMAResourceTranslation
                    {
                        Translation = new OAuthTranslation(Guid.NewGuid().ToString(), "première ressource", "fr")
                        {
                            Type = "description"
                        }
                    }
                },
                Scopes = new List<string>
                {
                    "read"
                },
                Subject = "sub",
                Type = "type"
            }
        };
    }
}