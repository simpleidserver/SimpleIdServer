// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Options;
using System;

namespace SimpleIdServer.IdServer.UI
{
    public class TOTPAuthenticator : OTPAuthenticator, IOTPAuthenticator
    {
        private readonly IdServerHostOptions _options;

        public TOTPAuthenticator(IOptions<IdServerHostOptions> options)
        {
            _options = options.Value;
        }

        public OTPAlgs Alg => OTPAlgs.TOTP;

        public long GenerateOtp(User oauthUser)
        {
            return GenerateOtp(oauthUser.GetOTPKey(), CalculateTimeStep(DateTime.UtcNow));
        }

        public long GenerateOtp(User oauthUser, DateTime date)
        {
            return GenerateOtp(oauthUser.GetOTPKey(), CalculateTimeStep(date));
        }

        public bool Verify(long otp, User user)
        {
            var currentDateTime = DateTime.UtcNow;
            var key = user.GetOTPKey();
            for (long offset = -1; offset <= 1; offset++)
            {
                var step = CalculateTimeStep(currentDateTime) + offset;
                if (GenerateOtp(key, step) == otp) return true;
            }

            return false;
        }

        private long CalculateTimeStep(DateTime dateTime)
        {
            var unixTimestamp = dateTime.ConvertToUnixTimestamp();
            var window = (long)unixTimestamp / (long)_options.TOTPStep;
            return window ;
        }
    }
}
