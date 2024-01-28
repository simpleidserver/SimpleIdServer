// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.Credential
{
    public class CredentialController : BaseController
    {
        private readonly IEnumerable<ICredentialFormatter> _formatters;
        private readonly ICredentialTemplateStore _credentialTemplateStore;
        private readonly IUserCredentialClaimStore _userCredentialClaimStore;
        private readonly CredentialIssuerOptions _options;

        public CredentialController(
            IEnumerable<ICredentialFormatter> formatters,
            ICredentialTemplateStore credentialTemplateStore,
            IUserCredentialClaimStore userCredentialClaimStore,
            IOptions<CredentialIssuerOptions> options)
        {
            _formatters = formatters;
            _credentialTemplateStore = credentialTemplateStore;
            _userCredentialClaimStore = userCredentialClaimStore;
            _options = options.Value;
        }

        [HttpPost(Constants.EndPoints.Credential)]
        [Authorize("Authenticated")]
        public async Task<IActionResult> Get([FromRoute] string prefix, [FromBody] CredentialRequest request, CancellationToken cancellationToken)
        {
            var subject = User.FindFirst("sub").Value;
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var validationResult = await Validate(request, cancellationToken);
            if (validationResult.ErrorResult != null) return Build(validationResult.ErrorResult.Value);
            var credentialTemplateClaims = validationResult.CredentialTemplate.Claims;
            var userCredentials = await _userCredentialClaimStore.Resolve(subject, credentialTemplateClaims);
            var formatter = validationResult.Formatter;
            var result = formatter.Build(new BuildCredentialRequest
            {
                JsonLdContext = validationResult.CredentialTemplate.JsonLdContext,
                Type = validationResult.CredentialTemplate.Id,
                Subject = subject,
                UserCredentialClaims = userCredentials,
                // ValidFrom - from credential template ??
                // ValidUntil - from credential template ??
                Id = Guid.NewGuid().ToString(), // What is the identifier ??? // https://www.w3.org/TR/vc-data-model-2.0/#identifiers
                Issuer = issuer
            }, _options.DidDocument, _options.VerificationMethodId);

            return new OkObjectResult(new CredentialResult
            {
                Format = validationResult.CredentialTemplate.Format,
                Credential = result
            });
        }

        private async Task<CredentialValidationResult> Validate(CredentialRequest credentialRequest, CancellationToken cancellationToken)
        {
            if (credentialRequest == null)
            {
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST));
            }

            var atCredentialIdentifiers = GetCredentialIdentifiers();
            if(atCredentialIdentifiers == null && string.IsNullOrWhiteSpace(credentialRequest.Format))
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Format)));


            if(credentialRequest.Proof != null)
            {
                // proof_type = jwt
                // proof_type = cwt

                // Proof.
            }

            if (atCredentialIdentifiers != null && string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier))
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.CredentialIdentifier)));

            if(!string.IsNullOrWhiteSpace(credentialRequest.Format) && !string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier))
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.CANNOT_USER_CREDENTIAL_IDENTIFIER_WITH_FORMAT));

            if(!atCredentialIdentifiers.Contains(credentialRequest.Credentialidentifier))
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.INVALID_CREDENTIAL_IDENTIFIER));

            ICredentialFormatter formatter = null;
            CredentialConfiguration credentialTemplate = null;
            if (!string.IsNullOrWhiteSpace(credentialRequest.Format))
            {
                formatter = _formatters.SingleOrDefault(f => f.Format == credentialRequest.Format);
                if (formatter == null) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.UNSUPPORTED_CREDENTIAL_FORMAT, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL_FORMAT, credentialRequest.Format)));
                var header = formatter.ExtractHeader(credentialRequest.Data);
                if (header == null) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.CREDENTIAL_TYPE_CANNOT_BE_EXTRACTED));
                credentialTemplate = await _credentialTemplateStore.Get(header.Type, cancellationToken);
                if (credentialTemplate == null) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.UNSUPPORTED_CREDENTIAL_TYPE, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL_TYPE, header.Type)));
            }

            return CredentialValidationResult.Ok(formatter, credentialTemplate);
        }

        private class CredentialValidationResult : BaseValidationResult
        {
            private CredentialValidationResult(ErrorResult error) : base(error)
            {
            }

            private CredentialValidationResult(ICredentialFormatter formatter, CredentialConfiguration credentialTemplate)
            {
                Formatter = formatter;
                CredentialTemplate = credentialTemplate;
            }

            public ICredentialFormatter Formatter { get; private set; }

            public CredentialConfiguration CredentialTemplate { get; private set; }

            public static CredentialValidationResult Ok(ICredentialFormatter formatter, CredentialConfiguration credentialTemplate) => new CredentialValidationResult(formatter, credentialTemplate);

            public static CredentialValidationResult Error(ErrorResult error) => new CredentialValidationResult(error);
        }

        private List<string> GetCredentialIdentifiers()
        {
            var claim = User.Claims.SingleOrDefault(c => c.Type == "authorization_details");
            if (claim == null) return null;
            var jsonObj = JsonObject.Parse(claim.Value).AsObject();
            if (jsonObj.ContainsKey("credential_identifiers")) return null;
            return (jsonObj["credential_identifiers"] as JsonArray).Select(c => c.ToString()).ToList();
        }
    }
}
