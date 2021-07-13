// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.UI.Authenticate.Sms
{
    public class SmsHostOptions
    {
        public SmsHostOptions()
        {
            Message = "the confirmation code is {0}";
        }

        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromPhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
