// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.UI.Authenticate.Email
{
    public class EmailHostOptions
    {
        public EmailHostOptions()
        {
            HttpBody = "the confirmation code is {0}";
            Subject = "Confirmation code";
            SmtpHost = "smtp.gmail.com";
            SmtpPort = 587;
            SmtpEnableSsl = true;
        }

        public string HttpBody { get; set; }
        public string Subject { get; set; }
        public string FromEmail { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpEnableSsl { get; set; }
    }
}
