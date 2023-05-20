// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.CredentialIssuer.Parsers;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential
{
    public class CredentialController : BaseController
    {
        private readonly IEnumerable<ICredentialRequestParser> _parsers;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CredentialController> _logger;

        public CredentialController(IEnumerable<ICredentialRequestParser> parsers, IGrantedTokenHelper grantedTokenHelper, IUserRepository userRepository, ILogger<CredentialController> logger)
        {
            _parsers = parsers;
            _grantedTokenHelper = grantedTokenHelper;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromRoute] string prefix, [FromBody] CredentialRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
            try
            {
                var accessToken = ExtractBearerToken();
                var token = await _grantedTokenHelper.GetAccessToken(accessToken, cancellationToken);
                if (token == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, ErrorMessages.UNKNOWN_ACCESS_TOKEN);
                ValidateRequest(request);
                return null;
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
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
