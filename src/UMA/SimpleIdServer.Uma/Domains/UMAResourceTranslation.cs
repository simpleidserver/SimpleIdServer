// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAResourceTranslation : ICloneable
    {
        public OAuthTranslation Translation { get; set; }

        public object Clone()
        {
            return new UMAResourceTranslation
            {
                Translation = (OAuthTranslation)Translation.Clone()
            };
        }
    }
}
