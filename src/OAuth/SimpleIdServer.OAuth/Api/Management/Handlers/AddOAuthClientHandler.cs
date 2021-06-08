// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IAddOAuthClientHandler
    {
        Task<string> Handle(string language, JObject jObj, CancellationToken cancellationToken);
    }

    public class AddOAuthClientHandler : IAddOAuthClientHandler
    {
        public Task<string> Handle(string language, JObject jObj, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
