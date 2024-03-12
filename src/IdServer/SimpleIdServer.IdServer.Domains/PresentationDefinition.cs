// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains;

public class PresentationDefinition
{
    /// <summary>
    /// The string SHOULD provide a unique ID for the desired context.
    /// </summary>
    public string Id { get; set; } = null!;
    /// <summary>
    /// If present, its value SHOULD be a human-friendly string intended to constitute a distinctive designation of the Presentation Definition.
    /// </summary>
    public string? Name { get; set; } = null;
    /// <summary>
    /// If present, its value MUST be a string that describes the purpose for which the Presentation Definition's inputs are being used for.
    /// </summary>
    public string? Purpose { get; set; } = null;
}