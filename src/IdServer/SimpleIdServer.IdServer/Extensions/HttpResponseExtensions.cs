// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace Microsoft.AspNetCore.Http
{
    public static class HttpResponseExtensions
    {
        public static void SetNoCache(this HttpResponse response)
        {
            response.Headers.Add("Cache-Control", "no-store");
            response.Headers.Add("Pragma", "no-cache");
        }
    }
}
