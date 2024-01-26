// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public class CredentialSubjectClaim
{
    public int Level { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}