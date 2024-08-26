// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vp.Models;

namespace SimpleIdServer.Vp;

public class VpBuilderResult
{
    private VpBuilderResult(VerifiablePresentation vp, PresentationSubmission presentationSubmission)
    {
        Vp = vp;
        PresentationSubmission = presentationSubmission;
    }

    private VpBuilderResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public VerifiablePresentation Vp { get; private set; }

    public PresentationSubmission PresentationSubmission { get; private set; }

    public string ErrorMessage { get; private set; }

    public bool HasError
    {
        get
        {
            return ErrorMessage != null;
        }
    }

    public static VpBuilderResult Ok(VerifiablePresentation vp, PresentationSubmission presentationSubmission) => new VpBuilderResult(vp, presentationSubmission);

    public static VpBuilderResult Nok(string errorMessage) => new VpBuilderResult(errorMessage);
}
