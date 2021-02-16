// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.Jwt.Jwe;
using SimpleIdServer.Jwt.Jwe.CEKHandlers;
using SimpleIdServer.Jwt.Jwe.EncHandlers;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.Jwt.Jws.Handlers;

namespace SimpleIdServer.Jwt
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwt(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IJweGenerator, JweGenerator>();
            serviceCollection.AddTransient<IJwsGenerator, JwsGenerator>();
            serviceCollection.AddTransient<IEncHandler, A128CBCHS256EncHandler>();
            serviceCollection.AddTransient<IEncHandler, A192CBCHS384EncHandler>();
            serviceCollection.AddTransient<IEncHandler, A256CBCHS512EncHandler>();
            serviceCollection.AddTransient<ICEKHandler, RSA15CEKHandler>();
            serviceCollection.AddTransient<ICEKHandler, RSAOAEP256CEKHandler>();
            serviceCollection.AddTransient<ICEKHandler, RSAOAEPCEKHandler>();
            serviceCollection.AddTransient<ISignHandler, ECDSAP256SignHandler>();
            serviceCollection.AddTransient<ISignHandler, ECDSAP384SignHandler>();
            serviceCollection.AddTransient<ISignHandler, ECDSAP512SignHandler>();
            serviceCollection.AddTransient<ISignHandler, HMAC256SignHandler>();
            serviceCollection.AddTransient<ISignHandler, HMAC384SignHandler>();
            serviceCollection.AddTransient<ISignHandler, HMAC512SignHandler>();
            serviceCollection.AddTransient<ISignHandler, RSA256SignHandler>();
            serviceCollection.AddTransient<ISignHandler, RSA384SignHandler>();
            serviceCollection.AddTransient<ISignHandler, RSA512SignHandler>();
            serviceCollection.AddTransient<ISignHandler, PS256SignHandler>();
            serviceCollection.AddTransient<ISignHandler, PS384SignHandler>();
            serviceCollection.AddTransient<ISignHandler, PS512SignHandler>();
            serviceCollection.AddTransient<ISignHandler, NoneSignHandler>();
            return serviceCollection;
        }
    }
}
