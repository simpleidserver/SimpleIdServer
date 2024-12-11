using FormBuilder.EF.Configurations;
using FormBuilder.Models;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.EF;

public class FormBuilderDbContext : DbContext
{
    public FormBuilderDbContext(DbContextOptions<FormBuilderDbContext> options) : base(options)
    {
        
    }

    public DbSet<WorkflowRecord> Workflows { get; set; }
    public DbSet<FormRecord> Forms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new WorkflowRecordConfiguration());
        modelBuilder.ApplyConfiguration(new WorkflowStepConfiguration());
        modelBuilder.ApplyConfiguration(new WorkflowLinkConfiguration());
        modelBuilder.ApplyConfiguration(new FormRecordConfiguration());
    }
}
