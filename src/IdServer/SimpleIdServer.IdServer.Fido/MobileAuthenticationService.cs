// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Fido
{
    public class MobileAuthenticationService : IAuthenticationMethodService
    {
        public string Amr => Constants.MobileAMR;
        public string Name => "Mobile";
        public Type? OptionsType => typeof(FidoOptions);
    }
}
