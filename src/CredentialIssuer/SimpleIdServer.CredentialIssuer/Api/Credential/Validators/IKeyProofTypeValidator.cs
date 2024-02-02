// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.Credential.Validators;

public interface IKeyProofTypeValidator
{
    string Type { get; }
    Task<KeyProofTypeValidationResult> Validate(CredentialProofRequest request, CancellationToken cancellationToken);
}

public record KeyProofTypeValidationResult
{
    private KeyProofTypeValidationResult()
    {
        
    }

    public KeyProofTypeValidationResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
        IsValid = false;
    }

    public string CNonce { get; private set; }
    public string Subject { get; private set; }
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; }

    public static KeyProofTypeValidationResult Ok(string cNonce) => new KeyProofTypeValidationResult
    {
        IsValid = true,
        CNonce = cNonce
    };

    public static KeyProofTypeValidationResult Ok(string cNonce, string subject) => new KeyProofTypeValidationResult
    {
        IsValid = true,
        CNonce = cNonce,
        Subject = subject
    };
    
    public static KeyProofTypeValidationResult Error(string errorMessage) => new KeyProofTypeValidationResult(errorMessage: errorMessage);
}