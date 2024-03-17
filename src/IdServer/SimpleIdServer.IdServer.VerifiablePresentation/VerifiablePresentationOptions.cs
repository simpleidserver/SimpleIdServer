// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.VerifiablePresentation
{
    public class VerifiablePresentationOptions
    {
        public int SlidingExpirationTimeVpOfferMs { get; set; } = 30 * 1000;
    }
}
