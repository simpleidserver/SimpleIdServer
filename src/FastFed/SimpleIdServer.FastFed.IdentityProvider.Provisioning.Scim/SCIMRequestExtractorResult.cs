// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public class SCIMRequestExtractorResult
{
    private SCIMRequestExtractorResult(JsonObject result)
    {
        Result = result;
    }

    private SCIMRequestExtractorResult(List<string> errorMessages)
    {
        ErrorMessages = errorMessages;
    }

    public bool HasError
    {
        get
        {
            return ErrorMessages.Any();
        }
    }

    public List<string> ErrorMessages { get; private set; } = new List<string>();

    public JsonObject Result { get; private set; }

    public static SCIMRequestExtractorResult Ok(JsonObject result) => new SCIMRequestExtractorResult(result);

    public static SCIMRequestExtractorResult Error(List<string> errorMessages) => new SCIMRequestExtractorResult(errorMessages);

}