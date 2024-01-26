// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats.Models;

/// <summary>
/// https://www.w3.org/2018/credentials/#sotd
/// </summary>
public class W3CVerifiableCredential
{
    public string Id { get; set; }
    public List<string> Type { get; set; } = new List<string>();
    public string Issuer { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public W3CCredentialSubject CredentialSubject { get; set; }
}

public class W3CCredentialSubject
{
    public string Id { get; set; }
    public Dictionary<string, JsonObject> Claims { get; set; }
}