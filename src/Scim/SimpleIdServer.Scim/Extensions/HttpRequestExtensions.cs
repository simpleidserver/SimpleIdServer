// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetAbsoluteUriWithVirtualPath(this HttpRequest requestMessage)
        {
            var host = requestMessage.Host.Value;
            var http = "http://";
            if (requestMessage.IsHttps)
            {
                http = "https://";
            }

            var relativePath = requestMessage.PathBase.Value;
            return http + host + relativePath;
        }

        public static string GetToken(this HttpRequest requestMessage)
        {
            if (!requestMessage.Headers.ContainsKey("Authorization"))
            {
                return string.Empty;
            }

            var authStr = requestMessage.Headers["Authorization"];
            if (authStr.Count() != 1)
            {
                return string.Empty;
            }

            var splitted = authStr[0].Split(' ');
            if (splitted[0] != "Bearer")
            {
                return string.Empty;
            }

            return splitted[1];
        }

        public static string GetRelativePath(this HttpRequest requestMessage)
        {
            return requestMessage.PathBase.Value;
        }
    }
}
