// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Domains
{
    public static class UserExtensions
    {
        public static byte[] GetOTPKey(this User user) => user.OTPKey.ConvertToBase32();
    }
}
