// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Did.Models;

[Flags]
public enum VerificationMethodUsages
{
    AUTHENTICATION = 1,
    ASSERTION_METHOD = 2,
    KEY_AGREEMENT = 4,
    CAPABILITY_INVOCATION = 8,
    CAPABILITY_DELEGATION = 16
}
