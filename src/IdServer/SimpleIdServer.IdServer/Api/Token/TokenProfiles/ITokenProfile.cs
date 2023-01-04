// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Api.Token.TokenProfiles
{
    public interface ITokenProfile
    {
        string Profile { get; }
        void Enrich(HandlerContext context);
    }
}