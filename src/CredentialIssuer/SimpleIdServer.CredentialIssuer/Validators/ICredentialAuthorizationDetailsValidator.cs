// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.CredentialIssuer.Validators
{
    public interface ICredentialAuthorizationDetailsValidator
    {
        string Format { get; }
        void Validate(AuthorizationData authorizationData);
    }
}
