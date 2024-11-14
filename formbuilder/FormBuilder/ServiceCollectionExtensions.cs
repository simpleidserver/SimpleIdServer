using FormBuilder;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Helpers;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFormBuilder(this IServiceCollection services)
    {
        services.AddTransient<IFormElementDefinition, FormInputFieldDefinition>();
        services.AddTransient<IFormElementDefinition, FormPasswordFieldDefinition>();
        services.AddTransient<IFormElementDefinition, FormButtonDefinition>();
        services.AddTransient<IFormElementDefinition, FormStackLayoutDefinition>();
        services.AddTransient<IFormElementDefinition, FormCheckboxDefinition>();
        services.AddTransient<IFormElementDefinition, DividerLayoutDefinition>();
        services.AddTransient<IFormElementDefinition, FormAnchorDefinition>();
        services.AddTransient<ITranslationHelper, TranslationHelper>();
        services.AddTransient<IRenderFormElementsHelper, RenderFormElementsHelper>();
        return services;
    }
}
