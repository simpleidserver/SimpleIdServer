// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.UI
{
    public class HOTPAuthenticator : OTPAuthenticator, IOTPAuthenticator
    {
        public OTPAlgs Alg => OTPAlgs.HOTP;

        public string GenerateOtp(UserCredential credential)
        {
            var result = GenerateOtp(credential.OTPKey, credential.OTPCounter);
            credential.OTPCounter++;
            return result;
        }

        public bool Verify(string otp, UserCredential credential)
        {
            var key = credential.OTPKey;
            for(long i = credential.OTPCounter - credential.HOTPWindow; i <= credential.OTPCounter; i++)
            {
                if (GenerateOtp(key, i) == otp) return true;
            }

            for(long i = credential.OTPCounter + 1; i <= credential.OTPCounter + credential.HOTPWindow; i++)
            {
                if(GenerateOtp(key, i) == otp)
                {
                    credential.OTPCounter = (int)i;
                    return true;
                }
            }

            return false;
        }
    }
}
