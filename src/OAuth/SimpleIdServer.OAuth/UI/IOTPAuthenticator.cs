﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OAuth.UI
{
    public interface IOTPAuthenticator
    {
        OTPAlgs Alg { get; }
        long GenerateOtp(OAuthUser oauthUser);
        bool Verify(long otp, OAuthUser user);
    }
}
