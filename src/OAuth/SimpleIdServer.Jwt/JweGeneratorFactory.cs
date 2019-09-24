// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jwe;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;

namespace SimpleIdServer.Jwt
{
    public class JweGeneratorFactory
    {
        public IJweGenerator BuildJweGenerator()
        {
            return new JweGenerator(new IEncHandler[]
            {
                new A128CBCHS256EncHandler(),
                new A192CBCHS384EncHandler(),
                new A256CBCHS512EncHandler(),
            }, new ICEKHandler[]
            {
                new RSA15CEKHandler(),
                new RSAOAEP256CEKHandler(),
                new RSAOAEPCEKHandler()
            });
        }
    }
}