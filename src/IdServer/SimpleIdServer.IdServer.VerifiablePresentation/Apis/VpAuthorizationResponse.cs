// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis;

public class VpAuthorizationResponse
{
    [FromQuery(Name = "vp_token")]
    public string VpToken { get; set; }
    [FromQuery(Name = "state")]
    public string State { get; set; }
}