// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Saml.Idp
{
    public static class Constants
    {
        public const string CLIENT_TYPE = "SAML";

        public static class RouteNames
        {
            public const string Prefix = "saml";
            public const string Metadata = Prefix + "/metadata";
            public const string SingleSignOn = Prefix + "/SSO";
            public const string SingleSignOnHttpRedirect = SingleSignOn + "/Login";
            public const string SingleSignLogout = SingleSignOn + "/Logout";
            public const string SingleSignOnArtifact = SingleSignOn + "/Artifact";
        }
    }
}
