// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Identity;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.IdServer.Helpers;

public static class PasswordHelper
{
    public static bool VerifyHash(UserCredential credential, string pwd)
    {
        if(credential.HashAlg == PasswordHashAlgs.Default)
        {
            return VerifyDefaultHash(credential.Value, pwd);
        }
        else
        {
            return VerifyMicrosoftHash(credential.Value, pwd);
        }
    }

    public static string ComputerHash(UserCredential credential, string pwd)
    {
        if (credential.HashAlg == PasswordHashAlgs.Default)
        {
            return ComputeDefaultHash(pwd);
        }
        else
        {
            return ComputeMicrosoftHash(pwd);
        }
    }

    private static bool VerifyDefaultHash(string hash, string pwd)
    {
        var hashPayload = ComputeDefaultHash(pwd);
        return hash == hashPayload;
    }

    private static bool VerifyMicrosoftHash(string hash, string pwd)
    {
        var hasher = new PasswordHasher<User>();
        return hasher.VerifyHashedPassword(new User(), hash, pwd) == PasswordVerificationResult.Success;
    }

    private static string ComputeDefaultHash(string pwd)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashPayload = sha256.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            return Convert.ToBase64String(hashPayload);
        }
    }

    private static string ComputeMicrosoftHash(string pwd)
    {
        var hasher = new PasswordHasher<User>();
        return hasher.HashPassword(new User(), pwd);
    }
}
