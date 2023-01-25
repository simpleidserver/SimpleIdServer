// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace SimpleIdServer.IdServer.Sms
{
    public class IdServerSmsOptions
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromPhoneNumber { get; set; }
        public string Message { get; set; } = "the confirmation code is {0}";
    }
}
