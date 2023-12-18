// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Did.Models;

[Flags]
public enum DidDocumentVerificationMethodUsages
{
    AUTHENTICATION = 0,
    ASSERTION_METHOD = 1,
    KEY_AGREEMENT = 2,
    CAPABILITY_INVOCATION = 3,
    CAPABILITY_DELEGATION = 4
}
