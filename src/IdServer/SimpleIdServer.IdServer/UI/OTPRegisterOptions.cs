// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.UI;

public class IOTPRegisterOptions
{
    public OTPAlgs OTPAlg { get; set; } = OTPAlgs.TOTP;
    public string OTPValue { get; set; } = null;
    public int OTPCounter { get; set; } = 10;
    public string HttpBody { get; set; }
}
