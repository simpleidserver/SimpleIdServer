// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;

namespace SimpleIdServer.Scim.Helpers
{
    public class UriProvider : IUriProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UriProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetAbsoluteUriWithVirtualPath()
        {
            var requestMessage = _httpContextAccessor.HttpContext.Request;
            var host = requestMessage.Host.Value;
            var http = "http://";
            if (requestMessage.IsHttps)
            {
                http = "https://";
            }

            var relativePath = requestMessage.PathBase.Value;
            return http + host + relativePath;
        }
        public string GetRelativePath()
        {
            var requestMessage = _httpContextAccessor.HttpContext.Request;
            return requestMessage.PathBase.Value;
        }
    }
}