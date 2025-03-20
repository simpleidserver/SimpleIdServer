// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains;

public class DistributedCache
{
    public required string Id
    {
        get; set;
    }

    public required byte[] Value
    {
        get; set;
    }

    public required DateTimeOffset ExpiresAtTime
    {
        get; set;
    }

    public long? SlidingExpirationInSeconds
    {
        get; set;
    }

    public DateTimeOffset? AbsoluteExpiration
    {
        get; set;
    }
}
