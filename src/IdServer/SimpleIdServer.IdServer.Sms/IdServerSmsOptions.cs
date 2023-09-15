// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Sms
{
    public class IdServerSmsOptions
    {
        [ConfigurationRecord("Account SID", null, order: 0)]
        public string AccountSid { get; set; }
        [ConfigurationRecord("Auth Token", null, order: 1)]
        public string AuthToken { get; set; }
        [ConfigurationRecord("Phone number of the sender", null, order: 2)]
        public string FromPhoneNumber { get; set; }
        [ConfigurationRecord("Content of the message", null, order: 3)]
        public string Message { get; set; } = "the confirmation code is {0}";
    }
}
