// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.Uma.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Uma.Startup
{
    public class DefaultConfiguration
    {
        public static List<UMAResource> Resources = new List<UMAResource>
        {
            new UMAResource(Guid.NewGuid().ToString(), DateTime.UtcNow)
            {
                Descriptions = new List<OAuthTranslation>
                {
                    new OAuthTranslation(Guid.NewGuid().ToString(), "first resource", "en"),
                    new OAuthTranslation(Guid.NewGuid().ToString(), "première ressource", "fr")
                },
                Names = new List<OAuthTranslation>
                {
                    new OAuthTranslation(Guid.NewGuid().ToString(), "first resource", "en"),
                    new OAuthTranslation(Guid.NewGuid().ToString(), "première ressource", "fr")
                },
                Scopes = new List<string>
                {
                    "scope1",
                    "scope2"
                },
                Subject = "umaUser",
                Type = "type"
            },
            new UMAResource(Guid.NewGuid().ToString(), DateTime.UtcNow)
            {
                Descriptions = new List<OAuthTranslation>
                {
                    new OAuthTranslation(Guid.NewGuid().ToString(), "first resource", "en"),
                    new OAuthTranslation(Guid.NewGuid().ToString(), "première ressource", "fr")
                },
                Names = new List<OAuthTranslation>
                {
                    new OAuthTranslation(Guid.NewGuid().ToString(), "first resource", "en"),
                    new OAuthTranslation(Guid.NewGuid().ToString(), "première ressource", "fr")
                },
                Scopes = new List<string>
                {
                    "scope1",
                    "scope2"
                },
                Subject = "otherUser",
                Type = "type"
            }
        };

        public static List<UMAPendingRequest> PendingRequests = new List<UMAPendingRequest>
        {
            new UMAPendingRequest(Guid.NewGuid().ToString(), "umaUser", DateTime.UtcNow)
            {
                Requester = "requester",
                Resource = Resources.First(),
                Scopes = new List<string>
                {
                    "scope1"
                }
            },
            new UMAPendingRequest(Guid.NewGuid().ToString(), "otherUser", DateTime.UtcNow)
            {
                Requester = "umaUser",
                Resource = Resources.First(),
                Scopes = new List<string>
                {
                    "scope1"
                }
            }
        };
    }
}