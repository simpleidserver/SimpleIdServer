// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Extensions;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace SimpleIdServer.DID.Tests;

public class Ed25519SignatureKeyFixture
{
    [Test]
    public void When_SignAndCheck_Signature_ThenTrueIsReturned()
    {
        // ARRANGE
        var privateKey = "9e55d1e1aa1f455b8baad9fdf975503655f8b359d542fa7e4ce84106d625b35206fac1f22240cffd637ead6647188429fafda9c9cb7eae43386ac17f61115075";
        var plaintext = "thequickbrownfoxjumpedoverthelazyprogrammer";
        var key = Ed25519SignatureKey.From(null, privateKey.HexToByteArray());

        // ACT
        var sig = key.Sign(plaintext);

        // ASSERT
        Assert.That("1y_N9v6xI4DyG9vIuloivxm91EV96nDM3HXBUI4P2Owk0IxazqX63rQ5jlBih6tP_4H5QhkHHqbree7ExmTBCw", Is.EqualTo(sig));
    }

    [Test]
    public void When_Sign_Ed25519Signature2020_Then_Proof_Is_Correct()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "files", "unsignedCredential.json");
        var fileContent = File.ReadAllText(filePath);

        var publicKeyMulticodec = "z6MknCCLeeHBUaHu4aHSVLDCYQW9gjVJ7a63FpMvtuVMy53T";
        var privateKeyMulticodec = "zrv2EET2WWZ8T1Jbg4fEH5cQxhbUS22XxdweypUbjWVzv1YD6VqYuW6LH7heQCNYQCuoKaDwvv2qCWz3uBzG2xesqmf";
        var multicodecSerializer = MulticodecSerializerFactory.Build();
        var publicKey = multicodecSerializer.GetPublicKey(publicKeyMulticodec);
        var privateKey = multicodecSerializer.GetPrivateKey(privateKeyMulticodec);
        var signatureKey = Ed25519SignatureKey.From(publicKey, privateKey);

        var tmp = Path.Combine(Directory.GetCurrentDirectory(), $"{Guid.NewGuid().ToString()}.ttl");
        var store = new TripleStore();
        var parser = new JsonLdParser();
        parser.Load(store, new StringReader(fileContent));
        var graph = store.Graphs.First();

        // URDNA2015 ??? - RDF CANON - https://w3c.github.io/rdf-canon/spec/
        // Convert to RDF => Canonize in C14N
        // Canonize : https://github.com/digitalbazaar/jsonld-signatures/blob/567c8cf4479d2d8b6e65aa5e571c55d43573722b/lib/suites/LinkedDataSignature.js#L211
        RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
        System.IO.StringWriter sw = new System.IO.StringWriter();
        rdfxmlwriter.Save(graph, sw);
        String data = sw.ToString();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(data);
        XmlDsigC14NTransform t = new XmlDsigC14NTransform();
        t.LoadInput(xmlDoc);
        Stream s = (Stream)t.GetOutput(typeof(Stream));
        var xml = new StreamReader(s).ReadToEnd();

        var hash = Hash(System.Text.Encoding.UTF8.GetBytes(data));
        var str = Convert.ToBase64String(hash);

        // https://www.w3.org/TR/vc-data-model/#proofs-signatures
        // https://w3c.github.io/vc-data-integrity/#proofs
        // h3rECCckSyUOPziZaQqOXm4qj0pDap4P1y6Sxvj0OIQ=
        // UvURB+KqaP3Rw7utVYJOOt0RVd8HdXD9uT4U/bKQ+nM=

        /*
        var doc = JObject.Parse(fileContent);
        JsonLdProcessor
        var opts = new JsonLdOptions();
        var rdf = (RDFDataset)JsonLdProcessor.ToRDF(doc, opts);
        var serialized = RDFDatasetUtils.ToNQuads(rdf);

        // var tt = signatureKey.Sign(fileContent);
        */
        string ss = "";
    }

    private static byte[] Hash(byte[] payload)
    {
        byte[] result = null;
        using (var sha256 = SHA256.Create())
            result = sha256.ComputeHash(payload);

        return result;
    }
}
