// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentValidation;
using SimpleIdServer.SelfIdServer.Resources;

namespace SimpleIdServer.SelfIdServer.Api.Authorization;

public interface IAuthorizationRequestValidator
{

}

public class AuthorizationRequestValidator : AbstractValidator<AuthorizationRequest>
{
    public AuthorizationRequestValidator()
    {
        RuleFor(c => c.Nonce).NotNull().WithMessage(Global.NonceParameterRequired);
    }
}
