// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;

namespace SimpleIdServer.OAuth.Authenticate
{
    public class AuthenticateInstruction
    {
        public string ClientIdFromHttpRequestBody { get; set; }
        public string ClientSecretFromHttpRequestBody { get; set; }
        public string ClientIdFromAuthorizationHeader { get; set; }
        public string ClientSecretFromAuthorizationHeader { get; set; }
        public string ClientAssertionType { get; set; }
        public string ClientAssertion { get; set; }
        public JsonObject RequestData { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }
}