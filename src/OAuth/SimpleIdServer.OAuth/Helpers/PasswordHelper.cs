// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.OAuth.Helpers
{
    public static class PasswordHelper
    {
        public static string ComputeHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashPayload = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Encoding.UTF8.GetString(hashPayload);
            }
        }
    }
}
