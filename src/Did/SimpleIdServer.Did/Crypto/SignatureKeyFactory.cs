// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Did.Crypto;

public interface ISignatureKeyFactory
{

}

public class SignatureKeyFactory : ISignatureKeyFactory
{
    private readonly IEnumerable<IVerificationMethodFormatter> _formatters;

    public SignatureKeyFactory()
    {
        
    }

    public object Extract(
        DidDocumentVerificationMethod verificationMethod, 
        bool isMultibaseVerificationMethod)
    {
        if (verificationMethod == null) throw new ArgumentNullException(nameof(verificationMethod));
        return null;
    }
}
