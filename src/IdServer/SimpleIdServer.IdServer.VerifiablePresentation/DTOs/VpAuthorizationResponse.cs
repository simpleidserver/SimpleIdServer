// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.IdServer.VerifiablePresentation.DTOs;

public class VpAuthorizationResponse
{
    /// <summary>
    /// REQUIRED : JSON STRING or JSON OBJECT that must contain a single Verifiable Presentation or an array of JSON strings and JSON objects each of them containing a verifiable presentation.
    /// </summary>
    [FromQuery(Name = "vp_token")]
    public string VpToken { get; set; }
    [FromQuery(Name = "state")]
    public string State { get; set; }
    /// <summary>
    /// REQUIRED : contains mappings between the requested Verifiable Credentials and where to find them within the returned VP Token.
    /// </summary>
    [FromQuery(Name = "presentation_submission")]
    public string PresentationSubmission { get; set; }
}