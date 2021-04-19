// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;

namespace SimpleIdServer.OpenID.Helpers
{
    public class RequestObjectValidatorResult
    {
        public RequestObjectValidatorResult(JwsPayload jwsPayload, JwsHeader header)
        {
            JwsPayload = jwsPayload;
            JwsHeader = header;
        }

        public JwsPayload JwsPayload { get; private set; }
        public JwsHeader JwsHeader { get; private set; }
    }
}
