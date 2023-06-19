// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer
{
    public interface IAuthenticationMethodService
    {
        string Amr { get; }
    }

    public class PwdAuthenticationMethodService : IAuthenticationMethodService
    {
        public string Amr => Constants.Areas.Password;
    }
}
