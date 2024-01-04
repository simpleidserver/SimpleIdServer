// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using System;

namespace SimpleIdServer.Vc.Canonize;

public class RdfCanonize : ICanonize
{
    public string Name => NAME;

    public const string NAME = "RDF";

    public string Transform(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));
        var document = JObject.Parse(input);
        var rdf = (RDFDataset)JsonLdProcessor.ToRDF(document);
        var serialized = RDFDatasetUtils.ToNQuads(rdf);
        return serialized;
    }
}
