// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Did.Encoding;

public class MultibaseEncoding
{
    private static char specialChar = 'z';

    public static string Encode(byte[] payload)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        return $"{specialChar}{Encoding.Base58Encoding.Encode(payload)}";
    }

    public static byte[] Decode(string input) 
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        var value = input.TrimStart(specialChar);
        return Encoding.Base58Encoding.Decode(value);
    }
}
