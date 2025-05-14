// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.IdServer.Domains;

public class ClientSecret
{
    private string _value;

    public ClientSecret(string value)
    {
        _value = value;    
    }

    public string Sha256()
    {
        using (var sha256 = SHA256.Create())
        {
            var hashPayload = sha256.ComputeHash(Encoding.UTF8.GetBytes(_value));
            return Convert.ToBase64String(hashPayload);
        }
    }

    public string Sha512()
    {
        using (var sha512 = SHA512.Create())
        {
            var hashPayload = sha512.ComputeHash(Encoding.UTF8.GetBytes(_value));
            return Convert.ToBase64String(hashPayload);
        }
    }
}
