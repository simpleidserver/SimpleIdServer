// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.IO;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace SimpleIdServer.Vc.Canonize;

public class RdfCanonize : ICanonize
{
    public string Transform(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));
        var store = new TripleStore();
        var parser = new JsonLdParser();
        parser.Load(store, new StringReader(input));
        var nquadsWriter = new NQuadsWriter();
        var sw = new System.IO.StringWriter();
        nquadsWriter.Save(store, sw);
        return sw.ToString();
    }
}
