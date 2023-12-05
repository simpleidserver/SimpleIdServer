// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using SimpleIdServer.Configuration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI;

namespace SimpleIdServer.IdServer.Sms
{
    public class IdServerSmsOptions : IOTPRegisterOptions
    {
        [ConfigurationRecord("Account SID", null, order: 0)]
        public string AccountSid { get; set; }
        [ConfigurationRecord("Auth Token", null, order: 1)]
        public string AuthToken { get; set; }
        [ConfigurationRecord("Phone number of the sender", null, order: 2)]
        public string FromPhoneNumber { get; set; }
        [ConfigurationRecord("Content of the message", null, order: 3)]
        public string Message { get; set; } = "the confirmation code is {0}";
        [ConfigurationRecord("OTP Algorithm", null, order: 4)]
        public OTPTypes OTPType { get; set; } = OTPTypes.TOTP;
        [ConfigurationRecord("OTP Value", null, order: 5, isOTPValue: true)]
        public string OTPValue { get; set; } = null;
        [ConfigurationRecord("OTP Counter", null, order: 6, isOTPValue: true, displayCondition: "OTPType=HOTP")]
        public int OTPCounter { get; set; } = 10;
        [ConfigurationRecord("TOTP Step", null, order: 7, isOTPValue: true, displayCondition: "OTPType=TOTP")]
        public int TOTPStep { get; set; } = 30;
        [ConfigurationRecord("HOTP Window", null, order: 8, isOTPValue: true, displayCondition: "OTPType=HOTP")]
        public int HOTPWindow { get; set; } = 5;
        public OTPAlgs OTPAlg => (OTPAlgs)OTPType;
        public string HttpBody => Message;
    }

    public enum OTPTypes
    {
        [ConfigurationRecordEnum("HOTP")]
        HOTP = 0,
        [ConfigurationRecordEnum("TOTP")]
        TOTP = 1
    }
}
