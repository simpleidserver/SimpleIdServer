// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Net.Http;

namespace SimpleIdServer.OAuth.Infrastructures
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient();
    }
}
