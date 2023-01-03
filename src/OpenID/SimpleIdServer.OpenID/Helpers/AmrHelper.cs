// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Helpers
{
    public interface IAmrHelper
    {
        Task<AuthenticationContextClassReference> FetchDefaultAcr(IEnumerable<string> requestedAcrValues, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, Client client, CancellationToken cancellationToken);
        Task<AuthenticationContextClassReference> GetSupportedAcr(IEnumerable<string> requestedAcrValues, CancellationToken cancellationToken);
        string FetchNextAmr(AuthenticationContextClassReference acr, string currentAmr);
    }

    public class AmrHelper : IAmrHelper
    {
        private readonly IAuthenticationContextClassReferenceRepository _authenticationContextClassReferenceRepository;
        private readonly OAuthHostOptions _options;

        public AmrHelper(IAuthenticationContextClassReferenceRepository authenticationContextClassReferenceRepository, IOptions<OAuthHostOptions> options)
        {
            _authenticationContextClassReferenceRepository = authenticationContextClassReferenceRepository;
            _options = options.Value;
        }

        public async Task<AuthenticationContextClassReference> FetchDefaultAcr(IEnumerable<string> requestedAcrValues, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, Client client, CancellationToken cancellationToken)
        {
            var defaultAcr = await GetSupportedAcr(requestedAcrValues, cancellationToken);
            if (defaultAcr == null)
            {
                var acrClaim = requestedClaims.FirstOrDefault(r => r.Name == JwtRegisteredClaimNames.Acr);
                if (acrClaim != null)
                {
                    defaultAcr = await GetSupportedAcr(acrClaim.Values, cancellationToken);
                    if (defaultAcr == null && acrClaim.IsEssential)
                        throw new OAuthException(ErrorCodes.ACCESS_DENIED, ErrorMessages.NO_ESSENTIAL_ACR_IS_SUPPORTED);
                }

                if (defaultAcr == null)
                {
                    var acrs = new List<string>();
                    acrs.AddRange(client.GetDefaultAcrValues());
                    acrs.Add(_options.GetDefaultAcrValue());
                    defaultAcr = await GetSupportedAcr(acrs, cancellationToken);

                }
            }

            return defaultAcr;
        }

        public async Task<AuthenticationContextClassReference> GetSupportedAcr(IEnumerable<string> requestedAcrValues, CancellationToken cancellationToken)
        {
            var acrs = await _authenticationContextClassReferenceRepository.Query().AsNoTracking().Where(a => requestedAcrValues.Contains(a.Name)).ToListAsync(cancellationToken);
            foreach(var acrValue in requestedAcrValues)
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