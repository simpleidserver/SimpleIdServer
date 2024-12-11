using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace FormBuilder.EF.Configurations;

public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
{
    public void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Coordinate).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<Coordinate>(v, (JsonSerializerOptions)null));
        builder.Ignore(w => w.EltRef);
        builder.Ignore(w => w.Size);
    }
}
