// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did.Events
{
    public class VerificationMethodAdded : IEvent
    {
        public const string DEFAULT_NAME = "VerificationMethodAdded";
        public string Name => DEFAULT_NAME;
    }
}
