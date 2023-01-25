// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Email
{
    public class IdServerEmailOptions
    {
        public string HttpBody { get; set; } = "the confirmation code is {0}";
        public string Subject { get; set; } = "Confirmation code";
        public string? FromEmail { get; set; } = null;
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string? SmtpUserName { get; set; } = null;
        public string? SmtpPassword { get; set; } = null;
        public bool SmtpEnableSsl { get; set; } = true;
    }
}
