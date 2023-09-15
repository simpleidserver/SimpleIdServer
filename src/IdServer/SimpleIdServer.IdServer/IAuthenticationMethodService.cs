// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.IdServer
{
    public interface IAuthenticationMethodService
    {
        string Amr { get; }
        string Name { get; }
        Type? OptionsType { get; }
}

    public class PwdAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => Constants.Areas.Password;
        public string Name => "Password";
        public Type? OptionsType => null;
    }
}