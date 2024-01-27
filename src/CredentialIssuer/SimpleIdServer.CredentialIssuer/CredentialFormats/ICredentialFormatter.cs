// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public interface ICredentialFormatter
{
    public string Format { get; }
    CredentialHeader ExtractHeader(JsonObject jsonObj);
    JsonNode Build(BuildCredentialRequest request, DidDocument didDocument, string verificationMethodId);
}

public class BuildCredentialRequest
{
    public string Id { get; set; }
    public string Issuer { get; set; }
    public string Subject { get; set; }
    public string Type { get; set; }
    public string JsonLdContext { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public List<UserCredentialClaim> UserCredentialClaims { get; set; }
}