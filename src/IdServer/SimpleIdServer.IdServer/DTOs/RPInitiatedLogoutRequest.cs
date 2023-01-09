// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    public static class RPInitiatedLogoutRequest
    {
        public const string IdTokenHint = "id_token_hint";
        public const string PostLogoutRedirectUri = "post_logout_redirect_uri";
        public const string State = "state";
    }
}
