// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.AspNetCore.Http
{
    public static class HttpRequestExtensions
    {
        public static string GetAbsoluteUriWithVirtualPath(this HttpRequest requestMessage)
        {
            var host = requestMessage.Host.Value;
            var http = requestMessage.IsHttps ? "https://" : "http://";
            var relativePath = requestMessage.PathBase.Value;
            return http + host + relativePath;
        }

        public static string GetAbsoluteUriWithVirtualPath(this HttpRequest requestMessage, string controller, string action) => $"{GetAbsoluteUriWithVirtualPath(requestMessage)}/{controller}/{action}";
    }
}
