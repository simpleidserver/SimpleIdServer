// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace FormBuilder.EF.Configurations;

public class FormRecordConfiguration : IEntityTypeConfiguration<FormRecord>
{
    public void Configure(EntityTypeBuilder<FormRecord> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(w => w.SuccessMessageTranslations).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<List<FormMessageTranslation>>(v, (JsonSerializerOptions)null));
        builder.Property(w => w.ErrorMessageTranslations).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<List<FormMessageTranslation>>(v, (JsonSerializerOptions)null));
        builder.Property(w => w.Elements).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new ObservableCollection<IFormElementRecord>(JsonSerializer.Deserialize<List<IFormElementRecord>>(v, (JsonSerializerOptions)null)));
    }
}