// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace FormBuilder.EF.Configurations;

public class FormRecordConfiguration : IEntityTypeConfiguration<FormRecord>
{
    public void Configure(EntityTypeBuilder<FormRecord> builder)
    {
        var formElementComparer = new ValueComparer<ObservableCollection<IFormElementRecord>>(
            (c1, c2) => false,
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null && v.Id != null ? v.Id.GetHashCode() : 0)),
            c => c == null ? null : new ObservableCollection<IFormElementRecord>(c));
        var formMessageTranslationComparer = new ValueComparer<List<FormMessageTranslation>>(
            (c1, c2) => false,
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null && v.Key != null ? v.Key.GetHashCode() : 0)),
            c => c == null ? null : new List<FormMessageTranslation>(c));
        builder.HasKey(f => f.Id);
        builder.Property(w => w.SuccessMessageTranslations).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<List<FormMessageTranslation>>(v, (JsonSerializerOptions)null))
                .Metadata.SetValueComparer(formMessageTranslationComparer);
        builder.Property(w => w.ErrorMessageTranslations).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<List<FormMessageTranslation>>(v, (JsonSerializerOptions)null))
                .Metadata.SetValueComparer(formMessageTranslationComparer);
        builder.Property(w => w.Elements).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new ObservableCollection<IFormElementRecord>(JsonSerializer.Deserialize<List<IFormElementRecord>>(v, (JsonSerializerOptions)null)))
            .Metadata.SetValueComparer(formElementComparer);
    }
}