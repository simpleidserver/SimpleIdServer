// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IGetOTPCodeHandler
    {
        Task<long> Handle(string id, string claimName, CancellationToken cancellationToken);
    }

    public class GetOTPCodeHandler : IGetOTPCodeHandler
    {
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly ILogger<GetOTPCodeHandler> _logger;
        private readonly OAuthHostOptions _options;
        private readonly IEnumerable<IOTPAuthenticator> _otpAuthenticators;

        public GetOTPCodeHandler(
            IOAuthUserRepository oAuthUserRepository,
            ILogger<GetOTPCodeHandler> logger,
            IOptions<OAuthHostOptions> options,
            IEnumerable<IOTPAuthenticator> otpAuthenticators)
        {
            _oauthUserRepository = oAuthUserRepository;
            _logger = logger;
            _options = options.Value;
            _otpAuthenticators = otpAuthenticators;
        }

        public async Task<long> Handle(string id, string claimName, CancellationToken cancellationToken)
        {
            Domains.OAuthUser user;
            if (!string.IsNullOrWhiteSpace(claimName))
            {
                user = await _oauthUserRepository.FindOAuthUserByClaim(claimName, id, cancellationToken);
            }
            else
            {
                user = await _oauthUserRepository.FindOAuthUserByLogin(id, cancellationToken);
            }

            if (user == null)
            {
                _logger.LogError($"the user '{id}' doesn't exist");
                throw new OAuthUserNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_USER, id));
            }

            var authenticator = _otpAuthenticators.First(o => o.Alg == _options.OTPAlg);
            var otp = authenticator.GenerateOtp(user);
            if(_options.OTPAlg == Domains.OTPAlgs.HOTP)
            {
                user.IncrementCounter();
                await _oauthUserRepository.Update(user, cancellationToken);
                await _oauthUserRepository.SaveChanges(cancellationToken);
            }

            _logger.LogInformation($"OTP {otp} has been generated");
            return otp;
        }
    }
}
