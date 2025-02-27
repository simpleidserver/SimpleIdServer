// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;

namespace SimpleIdServer.Scim.Helpers;

public interface IRepresentationVersionBuilder
{
    string Build(SCIMRepresentation representation);
}

public class IncrementalRepresentationVersionBuilder : IRepresentationVersionBuilder
{
    public string Build(SCIMRepresentation representation)
    {
        int number;
        if (string.IsNullOrWhiteSpace(representation.Version) || !int.TryParse(representation.Version, out number))
        {
            return "1";
        }

        number++;
        return number.ToString();
    }
}
