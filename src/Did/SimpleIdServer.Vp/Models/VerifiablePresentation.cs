// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Vp.Models;

public class VerifiablePresentation
{
    /// <summary>
    /// The id property is optional and MAY be used to provide a unique identifier for the presentation. 
    /// </summary>
    public string? Id { get; set; } = null;
    /// <summary>
    /// The type property is required and expresses the type of presentation, such as VerifiablePresentation.
    /// </summary>
    public string Type { get; set; } = null!;
    /// <summary>
    /// If present, the value of the holder property is expected to be a URI for the entity that is generating the presentation.
    /// </summary>
    public string Holder { get; set; } = null;
}
