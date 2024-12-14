using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormBuilder.EF.Configurations;

public class WorkflowRecordConfiguration : IEntityTypeConfiguration<WorkflowRecord>
{
    public void Configure(EntityTypeBuilder<WorkflowRecord> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasMany(c => c.Steps).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.Links).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
