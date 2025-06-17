// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SimpleIdServer.IdServer.Store.EF.Extensions;

public static class PropertyBuilderExtensions
{
    public static void ConfigureSerializer(this PropertyBuilder<string[]> builder)
    {
        var stringArrayComparer = new ValueComparer<string[]>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToArray());
        builder.HasConversion(v => string.Join(',', v), v => v.Split(",", StringSplitOptions.RemoveEmptyEntries));
        builder.Metadata.SetValueComparer(stringArrayComparer);
    }

    public static void ConfigureSerializer(this PropertyBuilder<IEnumerable<string>> builder)
    {
        var stringArrayComparer = new ValueComparer<IEnumerable<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToArray());
        builder.HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        builder.Metadata.SetValueComparer(stringArrayComparer);
    }

    public static void ConfigureSerializer(this PropertyBuilder<ICollection<string>> builder)
    {
        var stringArrayComparer = new ValueComparer<ICollection<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToArray());
        builder.HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
        builder.Metadata.SetValueComparer(stringArrayComparer);
    }

    public static void ConfigureSerializer(this PropertyBuilder<List<string>> builder)
    {
        var stringArrayComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());
        builder.HasConversion(v => string.Join(',', v), v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        builder.Metadata.SetValueComparer(stringArrayComparer);
    }
}
