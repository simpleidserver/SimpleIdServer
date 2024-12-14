using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace FormBuilder.EF.Configurations;

public class WorkflowLinkConfiguration : IEntityTypeConfiguration<WorkflowLink>
{
    public void Configure(EntityTypeBuilder<WorkflowLink> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Ignore(w => w.SourceCoordinate);
        builder.Ignore(w => w.TargetCoordinate);
        builder.Ignore(w => w.IsLinkHoverStep);
        builder.Ignore(w => w.IsHover);
        builder.Property(w => w.Source).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<WorkflowLinkSource>(v, (JsonSerializerOptions)null));
    }
}
