// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Net;

namespace SimpleIdServer.OAuth.Client
{
    public class OAuthHttpResult
    {
        public OAuthHttpResult(HttpStatusCode statusCode, JObject content)
        {
            StatusCode = statusCode;
            Content = content;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public JObject Content { get; private set; }
    }
}
