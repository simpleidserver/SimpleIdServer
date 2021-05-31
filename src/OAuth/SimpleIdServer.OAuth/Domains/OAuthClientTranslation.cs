// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthClientTranslation : ICloneable
    {
        public OAuthTranslation Translation { get; set; }

        public object Clone()
        {
            return new OAuthClientTranslation
            {
                Translation = (OAuthTranslation)Translation.Clone()
            };
        }
    }
}
