// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SimpleIdServer.Saml.Helpers
{
    public static class Compression
    {
        public static string Decompress(string parameter)
        {
            if (TryDecompress(parameter, out string result))
            {
                return result;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(parameter));
        }

        public static bool TryDecompress(string parameter, out string result)
        {
            result = null;
            try
            {
                using (var originalStream = new MemoryStream(Convert.FromBase64String(parameter)))
                using (var decompressedStream = new MemoryStream())
                {
                    using (var deflateStream = new DeflateStream(originalStream, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(decompressedStream);
                    }

                    result = Encoding.UTF8.GetString(decompressedStream.ToArray());
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string Compress(string parameter)
        {
            using (var compressedStream = new MemoryStream())
            using (var deflateStream = new DeflateStream(compressedStream, CompressionLevel.Optimal))
            {
                using (var originalStream = new StreamWriter(deflateStream))
                {
                    originalStream.Write(parameter);
                }

                var buffer = compressedStream.GetBuffer();
                var lastIndex = Array.FindLastIndex(buffer, b => b != 0);
                Array.Resize(ref buffer, lastIndex + 1);
                return Convert.ToBase64String(buffer);
            }
        }
    }
}
