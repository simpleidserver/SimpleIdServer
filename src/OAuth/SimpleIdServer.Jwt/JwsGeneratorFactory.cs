// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.Jwt.Jws.Handlers;

namespace SimpleIdServer.Jwt
{
    public class JwsGeneratorFactory
    {
        public IJwsGenerator BuildJwsGenerator()
        {
            return new JwsGenerator(new ISignHandler[]
            {
                new ECDSAP256SignHandler(),
                new ECDSAP384SignHandler(),
                new ECDSAP512SignHandler(),
                new HMAC256SignHandler(),
                new HMAC384SignHandler(),
                new HMAC512SignHandler(),
                new RSA256SignHandler(),
                new RSA384SignHandler(),
                new RSA512SignHandler()
            });
        }
    }
}