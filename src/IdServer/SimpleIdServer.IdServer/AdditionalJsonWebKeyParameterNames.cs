// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer
{
    public static class AdditionalJsonWebKeyParameterNames
    {
        /// <summary>
        /// https://www.ietf.org/archive/id/draft-ietf-oauth-dpop-16.txt
        /// </summary>
        public const string Jkt = "jkt";
        /// <summary>
        /// The act (actor) claim provides a means within a JWT to express that delegation has occurred and identify the acting party to whom authority has been delegated.
        /// Actor claim.
        /// </summary>
        public const string Act = "act";
    }
}
