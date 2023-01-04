// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.IdServer.Domains
{
    public class UserCredential : ICloneable
    {
        public const string PWD = "pwd";

        public string CredentialType { get; set; } = null!;
        public string Value { get; set; } = null!;

        public object Clone()
        {
            return new UserCredential
            {
                CredentialType = CredentialType,
                Value = Value
            };
        }

        public static UserCredential CreatePassword(string pwd) => new UserCredential { CredentialType = "pwd", Value = PasswordHelper.ComputeHash(pwd) };

    }
}
