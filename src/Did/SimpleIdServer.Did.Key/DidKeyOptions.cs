// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Did.Key;

public class DidKeyOptions
{
    /// <summary>
    /// Verification method is extracted in a multibase format.
    /// </summary>
    public bool IsMultibaseVerificationMethod { get; set; } = true;
}
