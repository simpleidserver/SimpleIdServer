// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.FastFed.ApplicationProvider.Options; 

public class AuthOptions
{
    public string Authority { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public bool IgnoreCertificateError { get; set; } = false;
    public string AdministratorRole { get; set; } = "applicationProvider/administrator";
    public string Audience { get; set; } = "applicationProvider";
    public string AdministratorScope = "applicationProviderAdmin";
}