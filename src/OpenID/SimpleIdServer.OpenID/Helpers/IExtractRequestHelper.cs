// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Domains;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Helpers
{
    public interface IExtractRequestHelper
    {
        Task<JObject> Extract(string issuerName, JObject jObj, BaseClient oauthClient);
        Task<bool> Extract(HandlerContext context);
    }
}
