// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace SimpleIdServer.Vc.Models;

public enum ProofPurposeTypes
{
    [EnumMember(Value = "assertionMethod")]
    assertionMethod = 1
}