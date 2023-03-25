// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;

namespace SimpleIdServer.OpenIdConnect
{
    internal static class CustomHandlerRequestResults
    {
        internal static HandleRequestResult UnexpectedParams = HandleRequestResult.Fail("An OpenID Connect response cannot contain an identity token or an access token when using response_mode=query");
        internal static HandleRequestResult NoMessage = HandleRequestResult.Fail("No message.");
        internal static HandleRequestResult NoResponse = HandleRequestResult.Fail("Authorization Response doesn't contain a 'response' parameter");
        internal static HandleRequestResult ResponseSignatureIsInvalid = HandleRequestResult.Fail("signature of the 'response' parameter is not valid");
        internal static HandleRequestResult UnknownJWK = HandleRequestResult.Fail("The JSON Web Key cannot be extracted from the OPENID configuration to check the signature of the 'response' parameter");
    }
}
