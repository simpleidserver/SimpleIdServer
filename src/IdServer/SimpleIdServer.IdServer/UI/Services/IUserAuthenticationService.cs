// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI.Services;

public interface IUserAuthenticationService
{
    Task<CredentialsValidationResult> Validate(string realm, string authenticatedUserId, object viewModel, CancellationToken cancellationToken);
    Task<CredentialsValidationResult> Validate(string realm, User authenticatedUser, ICollection<Claim> claims, object viewModel, CancellationToken cancellationToken);
}

public enum ValidationStatus
{
    AUTHENTICATE = 0,
    INVALIDCREDENTIALS = 1,
    NOCONTENT = 2,
    UNKNOWN_USER = 3
}

public record CredentialsValidationResult
{
    private CredentialsValidationResult(User authenticatedUser, ICollection<Claim> claims)
    {
        AuthenticatedUser = authenticatedUser;
        Claims = claims;
        Status = ValidationStatus.AUTHENTICATE;
    }

    private CredentialsValidationResult(ValidationStatus status)
    {
        Status = status;
    }

    public User AuthenticatedUser { get; private set; }
    public ICollection<Claim> Claims { get; private set; }
    public ValidationStatus Status { get; private set; }
    public string ErrorCode { get; private set; }
    public string ErrorMessage { get; private set; }

    public static CredentialsValidationResult Ok(User user, ICollection<Claim> claims) => new CredentialsValidationResult(user, claims);

    public static CredentialsValidationResult Error(ValidationStatus status) => new CredentialsValidationResult(status);

    public static CredentialsValidationResult Error(string errorCode, string errorMessage) => new CredentialsValidationResult(ValidationStatus.NOCONTENT)
    {
        ErrorCode = errorCode,
        ErrorMessage = errorMessage
    };
}