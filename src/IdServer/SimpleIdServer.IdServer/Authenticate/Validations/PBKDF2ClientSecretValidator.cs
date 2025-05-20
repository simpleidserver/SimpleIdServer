// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Identity;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Authenticate.Validations;

public class PBKDF2ClientSecretValidator : IAlgClientSecretValidator
{
    public HashAlgs Alg => HashAlgs.PBKDF2;

    public bool IsValid(ClientSecret secret, string clientSecret)
    {
        var passwordHasher = new PasswordHasher<object>();
        var verifyResult = passwordHasher.VerifyHashedPassword(null, secret.Value, clientSecret);
        return verifyResult == PasswordVerificationResult.Success || verifyResult == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
