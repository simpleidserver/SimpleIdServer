// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace UseOpenIddictAsDatasource;

public class IdentityServerConfiguration
{
    public bool IsRealmEnabled { get; set; }
    public bool IsClientCertificateEnabled { get; set; }
    public ClientCertificateMode? ClientCertificateMode { get; set; }
    public string Authority { get; set; }
    public string SCIMBaseUrl { get; set; }
}
