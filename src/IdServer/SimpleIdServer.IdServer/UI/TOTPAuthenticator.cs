// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer.UI
{
    public class TOTPAuthenticator : OTPAuthenticator, IOTPAuthenticator
    {
        public OTPAlgs Alg => OTPAlgs.TOTP;

        public long GenerateOtp(UserCredential credential)
        {
            return GenerateOtp(credential.OTPKey, CalculateTimeStep(DateTime.UtcNow, credential));
        }

        public long GenerateOtp(UserCredential credential, DateTime date)
        {
            return GenerateOtp(credential.OTPKey, CalculateTimeStep(date, credential));
        }

        public bool Verify(long otp, UserCredential credential)
        {
            var currentDateTime = DateTime.UtcNow;
            var key = credential.OTPKey;
            for (long offset = -1; offset <= 1; offset++)
            {
                var step = CalculateTimeStep(currentDateTime, credential) + offset;
                if (GenerateOtp(key, step) == otp) return true;
            }

            return false;
        }

        private long CalculateTimeStep(DateTime dateTime, UserCredential credential)
        {
            var unixTimestamp = dateTime.ConvertToUnixTimestamp();
            var window = (long)unixTimestamp / (long)credential.TOTPStep;
            return window ;
        }
    }
}
