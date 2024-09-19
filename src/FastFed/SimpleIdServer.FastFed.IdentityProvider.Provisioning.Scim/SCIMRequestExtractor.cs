// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim.Extensions;
using SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim.Resources;
using SimpleIdServer.FastFed.Provisioning.Scim;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public class SCIMRequestExtractor
{
    public static SCIMRequestExtractorResult ExtractAdd(JsonObject jsonObject, SchemaGrammarDesiredAttributes attributes)
    {
        var errorMessages = new List<string>();
        var requiredUserAttributes = attributes.RequiredUserAttributes.Select(u => (SCIMFilterParser.Parse(u), u));
        var optionalUserAttributes = attributes.OptionalUserAttributes.Select(u => (SCIMFilterParser.Parse(u), u));
        var result = new JsonObject();
        Extract(result, jsonObject, requiredUserAttributes, errorMessages, true);
        Extract(result, jsonObject, optionalUserAttributes, errorMessages, false);
        if (errorMessages.Any()) return SCIMRequestExtractorResult.Error(errorMessages);
        return SCIMRequestExtractorResult.Ok(result);
    }

    private static void Extract(JsonObject result, JsonObject jsonObject, IEnumerable<(SCIMExpression, string)> filters, List<string> errorMessages, bool isRequired)
    {
        foreach(var filter in filters)
        {
            var parameter = jsonObject.Filter(filter.Item1, result);
            if(isRequired && parameter == null)
                errorMessages.Add(string.Format(Global.CannotExtractParameter, filter.Item2));
        }
    }
}
