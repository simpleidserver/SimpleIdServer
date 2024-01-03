// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace SimpleIdServer.Vc.Canonize;

public class RdfCanonize : ICanonize
{
    public string Name => NAME;

    public const string NAME = "RDF";

    public string Transform(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));
        var store = new TripleStore();
        var parser = new JsonLdParser();
        parser.Load(store, new StringReader(input));
        var nquadsWriter = new NQuadsWriter();
        var sw = new System.IO.StringWriter();
        nquadsWriter.Save(store, sw);
        var result = sw.ToString();
        return Sanitize(result);
    }

    private string Sanitize(string result)
    {
        result = result.Replace("^^<http://www.w3.org/2001/XMLSchema#string>", string.Empty);
        var splitted = result
            .Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();
        var newResult = new List<string>
        {
            splitted.Last()
        };
        for (var i = 0; i < splitted.Length - 1; i++)
            newResult.Add(splitted[i]);
        return string.Join(" ", newResult);
    }
}
