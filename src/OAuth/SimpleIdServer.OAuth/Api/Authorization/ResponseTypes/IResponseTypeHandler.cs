// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseTypes
{
    public interface IResponseTypeHandler
    {
        string GrantType { get; }
        string ResponseType { get; }
        int Order { get; }
        void Enrich(HandlerContext context);
    }
}