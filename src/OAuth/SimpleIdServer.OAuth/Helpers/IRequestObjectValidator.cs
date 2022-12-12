// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Helpers
{
    public interface IRequestObjectValidator
    {
        Task<RequestObjectValidatorResult> Validate(string request, Client oauthClient, CancellationToken cancellationToken, string errorCode = ErrorCodes.INVALID_REQUEST_OBJECT);
    }
}
