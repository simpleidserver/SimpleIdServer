// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Fido;

public class WebauthnAuthenticationService : IAuthenticationMethodService
{
    public string Amr => Constants.AMR;
    public string Name => "Web Authentication (Webauthn)";
    public Type? OptionsType => typeof(WebauthnOptions);
    public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.USERAUTHENTICATION | AuthenticationMethodCapabilities.USERREGISTRATION;
    public bool IsCredentialExists(User user) => user.Credentials.Any(c => c.CredentialType == Amr);
}
