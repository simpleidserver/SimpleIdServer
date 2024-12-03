using FormBuilder;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Factories;
using FormBuilder.Helpers;
using FormBuilder.Rules;
using FormBuilder.Services;
using FormBuilder.Transformers;
using FormBuilder.Url;
using Radzen;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFormBuilder(this IServiceCollection services, Action<FormBuilderOptions> cb = null)
    {
        if (cb == null) services.Configure<FormBuilderOptions>((c) => { });
        else services.Configure<FormBuilderOptions>(cb);
        services.AddTransient<IFormBuilderJsService, FormBuilderJsService>();

        services.AddTransient<IFormElementDefinition, FormInputFieldDefinition>();
        services.AddTransient<IFormElementDefinition, FormPasswordFieldDefinition>();
        services.AddTransient<IFormElementDefinition, FormButtonDefinition>();
        services.AddTransient<IFormElementDefinition, FormStackLayoutDefinition>();
        services.AddTransient<IFormElementDefinition, FormCheckboxDefinition>();
        services.AddTransient<IFormElementDefinition, DividerLayoutDefinition>();
        services.AddTransient<IFormElementDefinition, FormAnchorDefinition>();
        services.AddTransient<IFormElementDefinition, ListDataDefinition>();

        services.AddTransient<ITranslationHelper, TranslationHelper>();
        services.AddTransient<IRenderFormElementsHelper, RenderFormElementsHelper>();
        services.AddTransient<ITransformerFactory, TransformerFactory>();
        services.AddTransient<ITransformationRuleEngineFactory, TransformationRuleEngineFactory>();
        services.AddTransient<IRepetitionRuleEngineFactory, RepetitionRuleEngineFactory>();
        services.AddTransient<IFormElementDefinitionFactory, FormElementDefinitionFactory>();
        services.AddTransient<IFakerDataServiceFactory, FakerDataServiceFactory>();

        services.AddTransient<ITargetUrlHelper, DirectTargetUrlHelper>();
        services.AddTransient<ITargetUrlHelper, ControllerActionTargetUrlHelper>();
        services.AddTransient<ITargetUrlHelperFactory, TargetUrlHelperFactory>();

        services.AddTransient<ITransformer, ControllerActionTransformer>();

        services.AddTransient<ITransformationRuleEngine, IncomingTokensTransformationRuleEngine>();
        services.AddTransient<IRepetitionRuleEngine, IncomingTokensRepetitionRuleEngine>();
        services.AddTransient<IRuleEngine, RuleEngine>();

        services.AddTransient<IUriProvider, UriProvider>();

        services.AddScoped<DialogService>();

        services.AddHttpContextAccessor();
        services.AddHttpClient();
        return services;
    }
}
