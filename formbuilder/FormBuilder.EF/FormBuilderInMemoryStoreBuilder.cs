using FormBuilder.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FormBuilder.EF;

public class FormBuilderInMemoryStoreBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public FormBuilderInMemoryStoreBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public FormBuilderInMemoryStoreBuilder AddInMemoryWorkflows(List<WorkflowRecord> workflows)
    {
        var storeDbContext = _serviceProvider.GetService<FormBuilderDbContext>();
        if (!storeDbContext.Workflows.Any())
        {
            storeDbContext.Workflows.AddRange(workflows);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public FormBuilderInMemoryStoreBuilder AddInMemoryForms(List<FormRecord> forms)
    {
        var storeDbContext = _serviceProvider.GetService<FormBuilderDbContext>();
        if (!storeDbContext.Forms.Any())
        {
            storeDbContext.Forms.AddRange(forms);
            storeDbContext.SaveChanges();
        }

        return this;
    }
}
