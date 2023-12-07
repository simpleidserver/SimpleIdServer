// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.UI;

/// <summary>
/// OTP Values are used for password reset or during registration.
/// </summary>
public interface IOTPRegisterOptions
{
    public OTPAlgs OTPAlg { get; }
    public string OTPValue { get; set; }
    public int OTPCounter { get; set; }
    public int TOTPStep { get; set; }
    public int HOTPWindow { get; set; }
    public string HttpBody { get; }
}
