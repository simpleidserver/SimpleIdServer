// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OpenidFederation;

public static class OpenidFederationConstants
{
    public static class EndPoints
    {
        public const string OpenidFederation = ".well-known/openid-federation";
        public const string FederationFetch = "federation_fetch";
        public const string FederationList = "federation_list";
        public const string FederationRegistration = "federation_registration";
    }

    public const string EntityStatementContentType = "application/entity-statement+jwt";
}
