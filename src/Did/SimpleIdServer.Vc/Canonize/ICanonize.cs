// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Vc.Canonize;

/// <summary>
/// Documentation : https://www.w3.org/TR/vc-data-integrity/#transformations
/// </summary>
public interface ICanonize
{
    string Name { get; }
    string Transform(string input);
}