// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace FormBuilder.EF.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(w => w.Styles).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new List<TemplateStyle>(JsonSerializer.Deserialize<List<TemplateStyle>>(v, (JsonSerializerOptions)null)));
        builder.Property(w => w.Windows).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new List<TemplateWindow>(JsonSerializer.Deserialize<List<TemplateWindow>>(v, (JsonSerializerOptions)null)));
        builder.Property(w => w.Elements).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new List<TemplateElement>(JsonSerializer.Deserialize<List<TemplateElement>>(v, (JsonSerializerOptions)null)));
    }
}
