// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api.Token.Fetchers
{
    public interface IClaimTokenFormat
    {
        string Name { get; }
        Task<ClaimTokenFormatFetcherResult> Fetch(HandlerContext context);
        string GetSubjectName();
    }
}
