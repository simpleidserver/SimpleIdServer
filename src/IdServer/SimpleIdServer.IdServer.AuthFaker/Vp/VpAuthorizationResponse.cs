namespace SimpleIdServer.IdServer.AuthFaker.Vp;
// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

public class VpAuthorizationResponse
{
    public string VpToken { get; set; }
    public string State { get; set; }
    public string PresentationSubmission { get; set; }
}