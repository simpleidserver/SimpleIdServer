// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Helpers
{
    public class AmrHelper : IAmrHelper
    {
        private readonly IAuthenticationContextClassReferenceQueryRepository _authenticationContextClassReferenceRepository;
        private readonly OpenIDHostOptions _openidHostOptions;

        public AmrHelper(IAuthenticationContextClassReferenceQueryRepository authenticationContextClassReferenceRepository, IOptions<OpenIDHostOptions> openidHostOptions)
        {
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
            _openidHostOptions = openidHostOptions.Value;
        }

        public async Task<AuthenticationContextClassReference> FetchDefaultAcr(IEnumerable<string> requestedAcrValues, OpenIdClient openidClient)
        {
            var acrs = new List<string>();
            acrs.AddRange(requestedAcrValues);
            if (openidClient.DefaultAcrValues.Any())
            {
                acrs.AddRange(openidClient.DefaultAcrValues);
            }

            if (!string.IsNullOrWhiteSpace(_openidHostOptions.DefaultAcrValue))
            {
                acrs.Add(_openidHostOptions.DefaultAcrValue);
            }

            foreach(var acrValue in acrs)
            {
                var acr = await _authenticationContextClassReferenceRepository.FindACRByName(acrValue);
                if (acr != null)
                {
                    return acr;
                }
            }

            return null;
        }

        public string FetchNextAmr(AuthenticationContextClassReference acr, string currentAmr)
        {
            var index = acr.AuthenticationMethodReferences.ToList().IndexOf(currentAmr);
            if (index == -1 || (index + 1) >= acr.AuthenticationMethodReferences.Count())
            {
                return null;
            }

            return acr.AuthenticationMethodReferences.ElementAt(index + 1);
        }
    }
}