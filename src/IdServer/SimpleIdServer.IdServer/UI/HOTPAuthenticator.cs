// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;

namespace SimpleIdServer.IdServer.UI
{
    public class HOTPAuthenticator : OTPAuthenticator, IOTPAuthenticator
    {
        private readonly IdServerHostOptions _options;

        public HOTPAuthenticator(IOptions<IdServerHostOptions> options)
        {
            _options = options.Value;
        }

        public OTPAlgs Alg => OTPAlgs.HOTP;

        public long GenerateOtp(User user)
        {
            return GenerateOtp(user.GetOTPKey(), user.OTPCounter);
        }

        public bool Verify(long otp, User user)
        {
            var key = user.GetOTPKey();
            for(long i = user.OTPCounter - _options.HOTPWindow; i <= user.OTPCounter; i++)
            {
                if (GenerateOtp(key, i) == otp)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
