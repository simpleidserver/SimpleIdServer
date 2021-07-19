// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Xsd;
using System;
using System.Net;

namespace SimpleIdServer.Saml.Exceptions
{
    public class SamlException : Exception
    {
        public SamlException(HttpStatusCode httpStatus, string status, string statusMessage) : base(statusMessage)
        {
            HttpStatusCode = httpStatus;
            Status = status;
            StatusMessage = statusMessage;
        }

        public HttpStatusCode HttpStatusCode { get; private set; }
        public string Status { get; private set; }
        public string StatusMessage { get; private set; }

        public StatusResponseType BuildResponse()
        {
            return new ResponseType
            {
                ID = Guid.NewGuid().ToString(),
                IssueInstant = DateTime.UtcNow,
                Status = new StatusType
                {
                    StatusCode = new StatusCodeType
                    {
                        Value = Status
                    },
                    StatusMessage = StatusMessage
                }
            };
        }
    }
}
