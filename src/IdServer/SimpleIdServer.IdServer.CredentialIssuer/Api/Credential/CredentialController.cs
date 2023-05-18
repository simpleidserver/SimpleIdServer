// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.CredentialIssuer.Parsers;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential
{
    public class CredentialController : BaseController
    {
        private readonly IEnumerable<ICredentialRequestParser> _parsers;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IUserRepository _userRepository;

        public CredentialController(IEnumerable<ICredentialRequestParser> parsers, IJwtBuilder jwtBuilder, IUserRepository userRepository)
        {
            _parsers = parsers;
            _jwtBuilder = jwtBuilder;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromRoute] string prefix, [FromBody] CredentialRequest request)
        {
            prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
            var accessToken = ExtractBearerToken();
            var extractedJwt = _jwtBuilder.ReadSelfIssuedJsonWebToken(prefix, accessToken);
            if (extractedJwt.Error != null) return null;
            ValidateRequest(request);
            var sub = extractedJwt.Jwt.Subject;

            return null;
        }

        protected void ValidateRequest(CredentialRequest request)
        {
            if (request == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CREDENTIAL_REQUEST_INVALID);
            if (string.IsNullOrWhiteSpace(request.Format)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Format));
            var parser = _parsers.SingleOrDefault(p => p.Format == request.Format);
            if (parser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_FORMAT, request.Format));
        }
    }
}
