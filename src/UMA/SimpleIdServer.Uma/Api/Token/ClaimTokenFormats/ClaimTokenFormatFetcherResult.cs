// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;

namespace SimpleIdServer.Uma.Api.Token.Fetchers
{
    public class ClaimTokenFormatFetcherResult
    {
        public ClaimTokenFormatFetcherResult(string subject, JwsPayload payload)
        {
            Subject = subject;
            Payload = payload;
        }

        public string Subject { get; set; }
        public JwsPayload Payload { get; set; }
    }
}
