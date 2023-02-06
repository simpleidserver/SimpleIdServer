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

        public long GenerateOtp(UserCredential credential)
        {
            return GenerateOtp(credential.OTPKey, CalculateTimeStep(DateTime.UtcNow));
        }

        public long GenerateOtp(UserCredential credential, DateTime date)
        {
            return GenerateOtp(credential.OTPKey, CalculateTimeStep(date));
        }

        public bool Verify(long otp, UserCredential credential)
        {
            var currentDateTime = DateTime.UtcNow;
            var key = credential.OTPKey;
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
