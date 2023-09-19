// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer
{
    public interface IAuthenticationMethodService
    {
        string Amr { get; }
        string Name { get; }
        Type? OptionsType { get; }
        bool IsCredentialExists(User user);
}

    public class PwdAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => Constants.Areas.Password;
        public string Name => "Password";
        public Type? OptionsType => null;
        public bool IsCredentialExists(User user) => user.Credentials.Any(c => c.CredentialType == Constants.Areas.Password);
    }
}