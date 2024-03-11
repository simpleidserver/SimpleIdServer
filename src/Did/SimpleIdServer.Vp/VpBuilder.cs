// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vp.Models;

namespace SimpleIdServer.Vp;

public class VpBuilder
{
    private readonly VerifiablePresentation _verifiablePresentation;

    private VpBuilder(VerifiablePresentation verifiablePresentation)
    {
        _verifiablePresentation = verifiablePresentation;    
    }

    public static VpBuilder New(string id, string holder, string type = null, string jsonLdContext = null)
    {
        var record = new VerifiablePresentation
        {
            Id = id,
            Holder = holder
        };
        record.Context.Add(VpConstants.VerifiablePresentationContext);
        if (!string.IsNullOrWhiteSpace(jsonLdContext)) record.Context.Add(jsonLdContext);
        record.Type.Add(VpConstants.VerifiablePresentationType);
        if (!string.IsNullOrWhiteSpace(type)) record.Type.Add(type);
        return new VpBuilder(record);
    }

    public VpBuilder AddVerifiableCredential(W3CVerifiableCredential vc)
    {
        _verifiablePresentation.VerifiableCredential.Add(vc);
        return this;
    }

    public VerifiablePresentation Build() => _verifiablePresentation;
}