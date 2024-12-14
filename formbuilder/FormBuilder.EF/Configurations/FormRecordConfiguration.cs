using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace FormBuilder.EF.Configurations;

public class FormRecordConfiguration : IEntityTypeConfiguration<FormRecord>
{
    public void Configure(EntityTypeBuilder<FormRecord> builder)
    {
        builder.HasKey(f => f.Name);
        builder.Property(w => w.Elements).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new ObservableCollection<IFormElementRecord>(JsonSerializer.Deserialize<List<IFormElementRecord>>(v, (JsonSerializerOptions)null)));
    }
}