// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did;
using SimpleIdServer.Vc;
using SimpleIdServer.Vp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Vp;

public interface IVpVerifier
{
    Task Verify(VerifiablePresentation presentation, CancellationToken cancellationToken);
}

public class VpVerifier
{
    private readonly IDidFactoryResolver _didFactoryResolver;

    public VpVerifier(IDidFactoryResolver didFactoryResolver)
    {
        _didFactoryResolver = didFactoryResolver;
    }

    public async Task Verify(VerifiablePresentation presentation, CancellationToken cancellationToken)
    {
        /*
        var securedDocument = SecuredDocument.New();
        var holderDid = await _didFactoryResolver.Resolve(presentation.Holder, cancellationToken);
        if (!securedDocument.Check(presentation, holderDid)) throw new InvalidOperationException("verifiable presentation signature is not correct");
        foreach(var vc in presentation.VerifiableCredential)
        {
            var issuerDid = await _didFactoryResolver.Resolve(vc.Issuer, cancellationToken);
            if (!securedDocument.Check(vc, issuerDid))
                throw new InvalidOperationException($"verifiable credential {vc.Id} signature is not correct");
        }
        */
    }
}
