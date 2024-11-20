﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public interface ICredentialFormatter
{
    public string Format { get; }
    JsonObject ExtractCredentialIssuerMetadata(CredentialConfiguration configuration);
    CredentialHeader ExtractHeader(JsonObject jsonObj);
    JsonNode Build(BuildCredentialRequest request, DidDocument didDocument, string verificationMethodId, IAsymmetricKey asymmetricKey);
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
    public List<string> AdditionalTypes { get; set; }
    public CredentialSchema Schema { get; set; }
    public CredentialConfiguration CredentialConfiguration { get; set; }
    public List<CredentialUserClaimNode> UserClaims { get; set; }
}

public interface INode
{
    int Level { get; set; }
    string Name { get; set; }
}

public class CredentialUserClaimNode : INode
{
    public int Level { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}

public class CredentialSchema
{
    public string Id { get; set; }
    public string Type { get; set; }
}