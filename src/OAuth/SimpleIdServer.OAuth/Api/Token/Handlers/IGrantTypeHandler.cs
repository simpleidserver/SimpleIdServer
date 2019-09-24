// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.Handlers
{
    public interface IGrantTypeHandler
    {
        string GrantType { get; }
        Task<JObject> Handle(HandlerContext context);
    }
}