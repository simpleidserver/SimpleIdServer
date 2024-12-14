using Microsoft.Extensions.DependencyInjection;

namespace FormBuilder;

public class FormBuilderRegistration
{
    private readonly IServiceCollection _services;

    internal FormBuilderRegistration(IServiceCollection services)
    {
        _services = services;
    }

    public IServiceCollection Services => _services;
}
