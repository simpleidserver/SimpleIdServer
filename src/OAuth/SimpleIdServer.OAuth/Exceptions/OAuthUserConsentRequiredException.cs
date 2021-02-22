// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth.Exceptions
{
    public class OAuthUserConsentRequiredException : OAuthException
    {
        public OAuthUserConsentRequiredException() : base(string.Empty, string.Empty) { }

        public OAuthUserConsentRequiredException(string controllerName) : this()
        {
            ControllerName = controllerName;
        }

        public OAuthUserConsentRequiredException(string controllerName, string actionName) : this(controllerName) 
        {
            ActionName = actionName;
        }

        public string ControllerName { get; set; } = "Consents";
        public string ActionName { get; set; } = "Index";
    }
}
