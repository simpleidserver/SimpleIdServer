// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OpenidFederation.Clients;

public class OpenidTrustChainValidationResult
{
    public List<string> ErrorMessages { get; private set; } = new List<string>();

    public void AddError(string errorMessage)
        => ErrorMessages.Add(errorMessage);
}