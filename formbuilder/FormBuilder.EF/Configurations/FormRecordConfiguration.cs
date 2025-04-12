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
        builder.Property(w => w.Elements).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new ObservableCollection<IFormElementRecord>(JsonSerializer.Deserialize<List<IFormElementRecord>>(v, (JsonSerializerOptions)null)));
        builder.Property(w => w.Classes).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : new List<HtmlClassRecord>(JsonSerializer.Deserialize<List<HtmlClassRecord>>(v, (JsonSerializerOptions)null)));
        builder.HasMany(f => f.AvailableStyles).WithOne().HasForeignKey(f => f.FormId).OnDelete(DeleteBehavior.Cascade);
    }
}