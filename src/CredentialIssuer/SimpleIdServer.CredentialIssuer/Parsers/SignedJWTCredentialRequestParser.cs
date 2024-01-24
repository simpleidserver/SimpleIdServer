// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Credential;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Vc.DTOs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Parsers
{
    /// <summary>
    /// https://openid.net/specs/openid-4-verifiable-credential-issuance-1_0.html#section-e.1.1.5
    /// </summary>
    public class SignedJWTCredentialRequestParser : ICredentialRequestParser
    {
        private readonly ICredentialTemplateRepository _credentialTemplateRepository;

        public SignedJWTCredentialRequestParser(ICredentialTemplateRepository credentialTemplateRepository)
        {
            _credentialTemplateRepository = credentialTemplateRepository;
        }

        public string Format => Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials;

        public async Task<ExtractionResult> Extract(CredentialRequest request, CancellationToken cancellationToken)
        {
            if (request.OtherParameters == null || !request.OtherParameters.ContainsKey(W3CCredentialTemplateNames.Types)) return ExtractionResult.Error(string.Format(ErrorMessages.MISSING_PARAMETER, W3CCredentialTemplateNames.Types));
            var credentialTypes = request.GetTypes();
            var credentialTemplate = await _credentialTemplateRepository.Query().Include(c => c.Parameters).Include(c => c.ClaimMappers).AsNoTracking().FirstOrDefaultAsync(c => c.Parameters.All(p => p.Name == CredentialRequestNames.Types && credentialTypes.Contains(p.Value)), cancellationToken);
            if (credentialTemplate == null) return ExtractionResult.Error(ErrorMessages.NO_CREDENTIAL_FOUND);
            var result = new SignedJWTCredentialRequest
            {
                CredentialTypes = request.GetTypes()
            };
            return ExtractionResult.Ok(result, credentialTemplate);
        }
    }
}