// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI;

namespace SimpleIdServer.IdServer.Console;

public class IdServerConsoleOptions : IOTPRegisterOptions
{
    [ConfigurationRecord("Content of the message", null, order: 0)]
    public string HttpBody { get; set; } = "the confirmation code is {0}";
    [ConfigurationRecord("OTP Algorithm", null, order: 1)]
    public OTPTypes OTPType { get; set; } = OTPTypes.TOTP;
    [ConfigurationRecord("OTP Value", null, 2, null, CustomConfigurationRecordType.OTPVALUE)]
    public string OTPValue { get; set; } = null;
    [ConfigurationRecord("OTP Counter", null, 3, "OTPType=HOTP")]
    public int OTPCounter { get; set; } = 10;
    [ConfigurationRecord("TOTP Step", null, 4, "OTPType=TOTP")]
    public int TOTPStep { get; set; } = 30;
    [ConfigurationRecord("HOTP Window", null, 5, "OTPType=HOTP")]
    public int HOTPWindow { get; set; } = 5;
    public OTPAlgs OTPAlg => (OTPAlgs)OTPType;
}

public enum OTPTypes
{
    [ConfigurationRecordEnum("HOTP")]
    HOTP = 0,
    [ConfigurationRecordEnum("TOTP")]
    TOTP = 1
}
