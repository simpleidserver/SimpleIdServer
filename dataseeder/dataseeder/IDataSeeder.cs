// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace DataSeeder;

public interface IDataSeeder
{
    Task Apply(CancellationToken cancellationToken);
    bool IsBeforeDeployment { get; }
}