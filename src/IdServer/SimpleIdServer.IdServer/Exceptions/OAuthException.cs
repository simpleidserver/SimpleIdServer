// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Net;

namespace SimpleIdServer.IdServer.Exceptions
{
    public class OAuthException : Exception
    {
        public OAuthException(string code, string message) : base(message)
        {
            Code = code;
        }

        public OAuthException(HttpStatusCode statusCode, string code, string message) : base(message)
        {
            StatusCode = statusCode;
            Code = code;
        }

        public HttpStatusCode? StatusCode { get; set; } = null;
        public string Code { get; set; }
    }
}