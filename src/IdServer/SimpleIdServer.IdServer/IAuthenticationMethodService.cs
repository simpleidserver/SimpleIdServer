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
        AuthenticationMethodCapabilities Capabilities { get; }
        bool IsCredentialExists(User user);
    }

    [Flags]
    public enum AuthenticationMethodCapabilities
    {
        USERAUTHENTICATION = 1,
        PUSHNOTIFICATION = 2
    }

    public class PwdAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => Constants.Areas.Password;
        public string Name => "Password";
        public Type? OptionsType => null;
        public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.USERAUTHENTICATION;
        public bool IsCredentialExists(User user) => user.Credentials.Any(c => c.CredentialType == Constants.Areas.Password);
    }
}