// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.CredentialIssuer.Validators
{
    public interface IAuthorizationDetailsValidator
    {
        void Validate(ICollection<AuthorizationData> authDetails);
    }

    public class AuthorizationDetailsValidator : IAuthorizationDetailsValidator
    {
        private readonly IEnumerable<ICredentialAuthorizationDetailsValidator> _validators;

        public AuthorizationDetailsValidator(IEnumerable<ICredentialAuthorizationDetailsValidator> validators)
        {
            _validators = validators;
        }

        public void Validate(ICollection<AuthorizationData> authDetails)
        {
            var openidCredentials = authDetails.Where(t => t.Type == Constants.StandardAuthorizationDetails.OpenIdCredential);
            if (!openidCredentials.Any()) return;
            var missingFormat = openidCredentials.Any(t => t.GetFormat() == null);
            if (missingFormat) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_OPENID_CREDENTIAL_FORMAT);
            var allFormats = openidCredentials.Select(d => d.GetFormat()).Where(d => d != null).Distinct();
            var unexceptedFormats = allFormats.Where(f => !Vc.Constants.AllCredentialTemplateProfiles.Contains(f));
            if (unexceptedFormats.Any()) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIALS_FORMAT, string.Join(",", unexceptedFormats)));
            foreach(var openidCredential in openidCredentials)
            {
                var validator = _validators.Single(v => v.Format == openidCredential.GetFormat());
                validator.Validate(openidCredential);
            }
        }
    }
}
