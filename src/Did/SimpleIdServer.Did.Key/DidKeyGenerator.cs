// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Models;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Key;

public class DidKeyGenerator
{
    private IAsymmetricKey _key;
    private readonly string _format;
    private readonly IMulticodecSerializer _serializer;

    private DidKeyGenerator(string format, IMulticodecSerializer serializer)
    {
        _format = format;
        _serializer = serializer;
    }

    public static DidKeyGenerator New(string format = null)
    {
        var serializer = MulticodecSerializerFactory.Build();
        return new DidKeyGenerator(format, serializer);
    }

    public DidKeyGenerator SetSignatureKey(IAsymmetricKey key)
    {
        _key = key;
        return this;
    }

    public DidKeyGenerator GenerateRandomEd2559Key()
    {
        _key = Ed25519SignatureKey.Generate();
        return this;
    }

    public DidKeyGenerator GenerateRandomES256KKey()
    {
        _key = ES256KSignatureKey.Generate();
        return this;
    }

    public DidKeyExportResult Export(bool exportPrivateKey = false, bool isJsonEncoded = false)
    {
        if (_key == null) throw new InvalidOperationException("the key doesn't exist");
        string multicodec;
        if(!isJsonEncoded)
            multicodec = exportPrivateKey ? _serializer.SerializePrivateKey(_key) : _serializer.SerializePublicKey(_key);
        else
        {
            var securityKey = new JsonWebKeySecurityKey(exportPrivateKey ? _key.GetPrivateJwk() : _key.GetPublicJwk());
            multicodec = exportPrivateKey ? _serializer.SerializePrivateKey(securityKey) : _serializer.SerializePublicKey(securityKey);
        }

        var did = $"{Did.Constants.Scheme}:{Constants.Type}:{multicodec}";
        DidKeyOptions opts = null;
        if (!string.IsNullOrWhiteSpace(_format))
            opts = new DidKeyOptions
            {
                PublicKeyFormat = _format
            };
        var resolver = DidKeyResolver.New(opts);
        var document = resolver.Resolve(did, CancellationToken.None).Result;
        return new DidKeyExportResult(did, document, _key);
    }
}

public class DidKeyExportResult
{
    public DidKeyExportResult(string did, DidDocument document, IAsymmetricKey key)
    {
        Did = did;
        Document = document;
        Key = key;
    }

    public string Did { get; private set; }
    public DidDocument Document { get; private set; }
    public IAsymmetricKey Key { get; private set; }
}