// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.AuthSchemeProvider.Handlers
{
    public interface IGetAllAuthSchemeProvidersHandler
    {
        Task<JArray> Handle(CancellationToken cancellationToken);
    }

    public class GetAllAuthSchemeProvidersHandler : IGetAllAuthSchemeProvidersHandler
    {
        private readonly IAuthenticationSchemeProviderRepository _authenticationSchemeProviderRepository;

        public GetAllAuthSchemeProvidersHandler(IAuthenticationSchemeProviderRepository authenticationSchemeProviderRepository)
        {
            _authenticationSchemeProviderRepository = authenticationSchemeProviderRepository;
        }

        public async Task<JArray> Handle(CancellationToken cancellationToken)
        {
            var authSchemeProviders = await _authenticationSchemeProviderRepository.GetAll(cancellationToken);
            var result = new JArray();
            foreach(var authSchemeProvider in authSchemeProviders)
            {
                result.Add(authSchemeProvider.ToDto());
            }

            return result;
        }
    }
}
