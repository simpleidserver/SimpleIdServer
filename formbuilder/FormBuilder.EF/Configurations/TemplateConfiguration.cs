// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace FormBuilder.EF.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.HasKey(x => x.Id);
        var templateStyleComparer = new ValueComparer<List<TemplateStyle>>(
            (c1, c2) => false,
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null && v.Id != null ? v.Id.GetHashCode() : 0)),
            c => c == null ? null : new List<TemplateStyle>(c));
        var templateWindowComparer = new ValueComparer<List<TemplateWindow>>(
            (c1, c2) => false,
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null && v.Key != null ? v.Key.GetHashCode() : 0)),
            c => c == null ? null : new List<TemplateWindow>(c));
        var templateElementComparer = new ValueComparer<List<TemplateElement>>(
            (c1, c2) => false,
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null && v.Key != null ? v.Key.GetHashCode() : 0)),
            c => c == null ? null : new List<TemplateElement>(c));
        builder.Property(w => w.Styles).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new List<TemplateStyle>(JsonSerializer.Deserialize<List<TemplateStyle>>(v, (JsonSerializerOptions)null)))
            .Metadata.SetValueComparer(templateStyleComparer);
        builder.Property(w => w.Windows).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new List<TemplateWindow>(JsonSerializer.Deserialize<List<TemplateWindow>>(v, (JsonSerializerOptions)null)))
            .Metadata.SetValueComparer(templateWindowComparer);
        builder.Property(w => w.Elements).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new List<TemplateElement>(JsonSerializer.Deserialize<List<TemplateElement>>(v, (JsonSerializerOptions)null)))
            .Metadata.SetValueComparer(templateElementComparer);
    }
}
