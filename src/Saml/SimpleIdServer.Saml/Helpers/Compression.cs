// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;

namespace SimpleIdServer.Saml.Helpers
{
    public static class Compression
    {
        public static string Decompress(string parameter)
        {
            parameter = HttpUtility.UrlDecode(parameter);
            using (var originalStream = new MemoryStream(Convert.FromBase64String(parameter)))
            using (var decompressedStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(originalStream, CompressionMode.Decompress))
                {
                    deflateStream.CopyTo(decompressedStream);
                }

                return Encoding.UTF8.GetString(decompressedStream.ToArray());
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

                return HttpUtility.UrlEncode(Convert.ToBase64String(compressedStream.GetBuffer()));
            }
        }
    }
}
