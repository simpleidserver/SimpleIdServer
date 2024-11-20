// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IAmrHelper
    {
        Task<AuthenticationContextClassReference> FetchDefaultAcr(string realm, IEnumerable<string> requestedAcrValues, IEnumerable<AuthorizedClaim> requestedClaims, Client client, CancellationToken cancellationToken);
        Task<AuthenticationContextClassReference> GetSupportedAcr(string realm, IEnumerable<string> requestedAcrValues, CancellationToken cancellationToken);
        string FetchNextAmr(AuthenticationContextClassReference acr, string currentAmr);
    }

    public class AmrHelper : IAmrHelper
    {
        private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
        private readonly IdServerHostOptions _options;

        public AmrHelper(IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, IOptions<IdServerHostOptions> options)
        {
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
            _options = options.Value;
        }

        public async Task<AuthenticationContextClassReference> FetchDefaultAcr(
            string realm, 
            IEnumerable<string> requestedAcrValues, 
            IEnumerable<AuthorizedClaim> requestedClaims, 
            Client client, 
            CancellationToken cancellationToken)
        {
            var defaultAcr = await GetSupportedAcr(realm, requestedAcrValues, cancellationToken);
            if (defaultAcr == null)
            {
                var acrClaim = requestedClaims?.FirstOrDefault(r => r.Name == JwtRegisteredClaimNames.Acr);
                if (acrClaim != null)
                {
                    defaultAcr = await GetSupportedAcr(realm, acrClaim.Values, cancellationToken);
                    if (defaultAcr == null && acrClaim.IsEssential)
                        throw new OAuthException(ErrorCodes.ACCESS_DENIED, Global.NoEssentialAcrIsSupported);
                }

                if (defaultAcr == null)
                {
                    var acrs = new List<string>();
                    if(client != null && client.DefaultAcrValues != null)
                        acrs.AddRange(client.DefaultAcrValues);
                    acrs.Add(_options.DefaultAcrValue);
                    defaultAcr = await GetSupportedAcr(realm, acrs, cancellationToken);
                }
            }

            return defaultAcr;
        }

        public async Task<AuthenticationContextClassReference> GetSupportedAcr(string realm, IEnumerable<string> requestedAcrValues, CancellationToken cancellationToken)
        {
            var acrs = await _authenticationContextClassReferenceRepository.GetByNames(realm, requestedAcrValues.ToList(), cancellationToken);
            foreach (var acrValue in requestedAcrValues)
            {
                var acr = acrs.FirstOrDefault(a => a.Name == acrValue);
                if (acr != null)
                    return acr;
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
