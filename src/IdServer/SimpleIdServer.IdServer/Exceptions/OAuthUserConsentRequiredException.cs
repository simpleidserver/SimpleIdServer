// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Exceptions
{
    public class OAuthUserConsentRequiredException : OAuthException
    {
        public OAuthUserConsentRequiredException(string grantId = null) : base(string.Empty, string.Empty) 
        {
            GrantId = grantId;
        }

        public string ControllerName { get; set; } = "Consents";
        public string ActionName { get; set; } = "Index";
        public string? GrantId { get; set; } = null;
    }
}
