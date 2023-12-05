// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace SimpleIdServer.IdServer.Helpers;

public class ResetPasswordLink
{
    public ResetPasswordLink(string otpCode, string login, string realm)
    {
        OTPCode = otpCode;
        Login = login;
    }

    public string OTPCode { get; set; }
    public string Login { get; set; }
    public string Realm { get; set; }
}
