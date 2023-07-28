// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib.Cbor;
using Fido2NetLib.Objects;

namespace SimpleIdServer.IdServer.U2FClient
{
    public static class CborMapExtensions
    {
        public static void Add(this CborMap map, string key, COSE.Algorithm value) => map.Add(key, (int)value);

        public static void Add(this CborMap map, COSE.KeyCommonParameter key, COSE.KeyType value) => map.Add((int)key, (int)value);

        public static void Add(this CborMap map, COSE.KeyCommonParameter key, COSE.Algorithm value) => map.Add((int)key, (int)value);

        public static void Add(this CborMap map, COSE.KeyTypeParameter key, COSE.EllipticCurve value) => map.Add((int)key, (int)value);

        public static void Add(this CborMap map, COSE.KeyTypeParameter key, byte[] value) => map.Add((int)key, value);
    }
}
