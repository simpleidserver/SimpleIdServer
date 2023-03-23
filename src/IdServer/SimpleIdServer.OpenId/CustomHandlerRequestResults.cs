// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;

namespace SimpleIdServer.OpenId
{
    internal static class CustomHandlerRequestResults
    {
        internal static HandleRequestResult UnexpectedParams = HandleRequestResult.Fail("An OpenID Connect response cannot contain an identity token or an access token when using response_mode=query");
        internal static HandleRequestResult NoMessage = HandleRequestResult.Fail("No message.");
    }
}
