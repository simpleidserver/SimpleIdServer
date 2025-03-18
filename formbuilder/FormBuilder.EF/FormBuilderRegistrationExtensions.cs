using FormBuilder.EF;
using FormBuilder.EF.Stores;
using FormBuilder.Repositories;
using FormBuilder.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FormBuilder;

public static class FormBuilderRegistrationExtensions
{
    public static FormBuilderRegistration UseEf(this FormBuilderRegistration formBuilderRegistration, Action<DbContextOptionsBuilder>? action = null)
    {
        var lifetime = ServiceLifetime.Scoped;
        formBuilderRegistration.Services.AddTransient<IFormStore, FormStore>();
        formBuilderRegistration.Services.AddTransient<IWorkflowStore, WorkflowStore>();
        if (action != null) formBuilderRegistration.Services.AddDbContext<FormBuilderDbContext>(action, lifetime);
        else formBuilderRegistration.Services.AddDbContext<FormBuilderDbContext>(o => o.UseInMemoryDatabase("formBuilder"), lifetime);
        return formBuilderRegistration;
    }
}