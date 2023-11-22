// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Infastructures;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace SimpleIdServer.IdServer.Swagger;

public class SidApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
{
    private const string _defaultVersion = "v1";
    private readonly ISidEndpointStore _edpsStore;
    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
    private readonly IModelMetadataProvider _modelMetadataProvider;
    private readonly IInlineConstraintResolver _constraintResolver;
    private readonly MvcOptions _mvcOptions;
    private readonly RouteOptions _routeOptions;
    private ApiDescriptionGroupCollection? _apiDescriptionGroups;

    public SidApiDescriptionGroupCollectionProvider(
        ISidEndpointStore edpsStore,
        IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
        IModelMetadataProvider modelMetadataProvider,
        IInlineConstraintResolver constraintResolver,
        IOptions<MvcOptions> optionsAccessor,
        IOptions<RouteOptions> routeOptions)
    {

        _edpsStore = edpsStore;
        _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        _modelMetadataProvider = modelMetadataProvider;
        _constraintResolver = constraintResolver;
        _mvcOptions = optionsAccessor.Value;
        _routeOptions = routeOptions.Value;
    }

    public ApiDescriptionGroupCollection ApiDescriptionGroups
    {
        get
        {
            if (_apiDescriptionGroups != null)
            {
                return _apiDescriptionGroups;
            }

            var actionDescriptors = _actionDescriptorCollectionProvider.ActionDescriptors;
            _apiDescriptionGroups = GetCollection(actionDescriptors);
            return _apiDescriptionGroups;
        }
    }

    private ApiDescriptionGroupCollection GetCollection(ActionDescriptorCollection actionDescriptors)
    {
        var apiDescriptions = GetApiDescriptions(actionDescriptors.Items);
        var groups = apiDescriptions
            .GroupBy(d => d.GroupName)
            .Select(g => new ApiDescriptionGroup(g.Key, g.ToArray()))
            .ToArray();
        var result =  new ApiDescriptionGroupCollection(groups, actionDescriptors.Version);
        var items = result.Items.SelectMany(i => i.Items);
        return result;
    }

    private List<ApiDescription> GetApiDescriptions(IReadOnlyList<ActionDescriptor> actions)
    {
        var result = new List<ApiDescription>();
        foreach (var action in actions.OfType<ControllerActionDescriptor>())
        {
            // https://github.com/dotnet/aspnetcore/blob/c5b2625c3141d05027ecb490a4e2ef32e5f9a5ad/src/Mvc/Mvc.ApiExplorer/src/DefaultApiDescriptionProvider.cs#L23
            var routeEndpoints = _edpsStore.Routes.Where(e =>
                e.Pattern.Defaults["controller"].ToString() == action.ControllerName &&
                e.Pattern.Defaults["action"].ToString() == action.ActionName);
            if (!routeEndpoints.Any()) continue;

            var httpMethods = GetHttpMethods(action);
            foreach (var httpMethod in httpMethods)
            {
                foreach(var routeEndpoint in routeEndpoints)
                {
                    var record = CreateApiDescription(action, httpMethod, _defaultVersion, routeEndpoint);
                    if (!result.Any(r => r.ActionDescriptor.Id == record.ActionDescriptor.Id))
                        result.Add(record);
                }
            }
        }

        return result;
    }

    private ApiDescription CreateApiDescription(
        ControllerActionDescriptor action,
        string? httpMethod,
        string? groupName,
        SidConventionalRouteEntry route)
    {
        // https://github.com/dotnet/aspnetcore/blob/88f9a3686b212c9d5d692b88b6557d5c77fc6155/src/Mvc/Mvc.ApiExplorer/src/DefaultApiDescriptionProvider.cs#L91
        var parsedTemplate = ParseTemplate(route);
        var apiDescription = new ApiDescription()
        {
            ActionDescriptor = action,
            GroupName = groupName,
            HttpMethod = httpMethod,
            RelativePath = GetRelativePath(parsedTemplate),
        };

        var templateParameters = parsedTemplate?.Parameters?.ToList() ?? new List<TemplatePart>();
        var parameterContext = new SidApiParameterContext(_modelMetadataProvider, action, templateParameters);
        foreach (var parameter in GetParameters(parameterContext))
        {
            if (parameter.Name == Constants.Prefix) continue;
            apiDescription.ParameterDescriptions.Add(parameter);
        }

        // action.Parameters
        /*
        var apiResponseTypes = _responseTypeProvider.GetApiResponseTypes(action);
        foreach (var apiResponseType in apiResponseTypes)
        {
            apiDescription.SupportedResponseTypes.Add(apiResponseType);
        }
        */

        // It would be possible here to configure an action with multiple body parameters, in which case you
        // could end up with duplicate data.
        if (apiDescription.ParameterDescriptions.Count > 0)
        {
            // Get the most significant accepts metadata
            var acceptsMetadata = action.EndpointMetadata.OfType<IAcceptsMetadata>().LastOrDefault();
            var requestMetadataAttributes = GetRequestMetadataAttributes(action);

            var contentTypes = GetDeclaredContentTypes(requestMetadataAttributes, acceptsMetadata);
            foreach (var parameter in apiDescription.ParameterDescriptions)
            {
                if (parameter.Source == BindingSource.Body)
                {
                    // For request body bound parameters, determine the content types supported
                    // by input formatters.
                    var requestFormats = GetSupportedFormats(contentTypes, parameter.Type);
                    foreach (var format in requestFormats)
                    {
                        apiDescription.SupportedRequestFormats.Add(format);
                    }
                }
                else if (parameter.Source == BindingSource.FormFile)
                {
                    // Add all declared media types since FormFiles do not get processed by formatters.
                    foreach (var contentType in contentTypes)
                    {
                        apiDescription.SupportedRequestFormats.Add(new ApiRequestFormat
                        {
                            MediaType = contentType,
                        });
                    }
                }
            }
        }

        return apiDescription;
    }

    private IReadOnlyList<ApiRequestFormat> GetSupportedFormats(MediaTypeCollection contentTypes, Type type)
    {
        if (contentTypes.Count == 0)
        {
            contentTypes = new MediaTypeCollection
                {
                    (string)null!,
                };
        }

        var results = new List<ApiRequestFormat>();
        foreach (var contentType in contentTypes)
        {
            foreach (var formatter in _mvcOptions.InputFormatters)
            {
                if (formatter is IApiRequestFormatMetadataProvider requestFormatMetadataProvider)
                {
                    var supportedTypes = requestFormatMetadataProvider.GetSupportedContentTypes(contentType, type);

                    if (supportedTypes != null)
                    {
                        foreach (var supportedType in supportedTypes)
                        {
                            results.Add(new ApiRequestFormat()
                            {
                                Formatter = formatter,
                                MediaType = supportedType,
                            });
                        }
                    }
                }
            }
        }

        return results;
    }
    
    private IList<ApiParameterDescription> GetParameters(SidApiParameterContext context)
    {
        // First, get parameters from the model-binding/parameter-binding side of the world.
        if (context.ActionDescriptor.Parameters != null)
        {
            foreach (var actionParameter in context.ActionDescriptor.Parameters)
            {
                var visitor = new PseudoModelBindingVisitor(context, actionParameter);

                ModelMetadata metadata;
                if (actionParameter is ControllerParameterDescriptor controllerParameterDescriptor &&
                    _modelMetadataProvider is ModelMetadataProvider provider)
                {
                    // The default model metadata provider derives from ModelMetadataProvider
                    // and can therefore supply information about attributes applied to parameters.
                    metadata = provider.GetMetadataForParameter(controllerParameterDescriptor.ParameterInfo);
                }
                else
                {
                    // For backward compatibility, if there's a custom model metadata provider that
                    // only implements the older IModelMetadataProvider interface, access the more
                    // limited metadata information it supplies. In this scenario, validation attributes
                    // are not supported on parameters.
                    metadata = _modelMetadataProvider.GetMetadataForType(actionParameter.ParameterType);
                }

                var bindingContext = new ApiParameterDescriptionContext(
                    metadata,
                    actionParameter.BindingInfo,
                    propertyName: actionParameter.Name);
                visitor.WalkParameter(bindingContext);
            }
        }

        if (context.ActionDescriptor.BoundProperties != null)
        {
            foreach (var actionParameter in context.ActionDescriptor.BoundProperties)
            {
                var visitor = new PseudoModelBindingVisitor(context, actionParameter);
                var modelMetadata = context.MetadataProvider.GetMetadataForProperty(
                    containerType: context.ActionDescriptor.ControllerTypeInfo.AsType(),
                    propertyName: actionParameter.Name);

                var bindingContext = new ApiParameterDescriptionContext(
                    modelMetadata,
                    actionParameter.BindingInfo,
                    propertyName: actionParameter.Name);

                visitor.WalkParameter(bindingContext);
            }
        }

        for (var i = context.Results.Count - 1; i >= 0; i--)
        {
            // Remove any 'hidden' parameters. These are things that can't come from user input,
            // so they aren't worth showing.
            if (!context.Results[i].Source.IsFromRequest)
            {
                context.Results.RemoveAt(i);
            }
        }

        // Next, we want to join up any route parameters with those discovered from the action's parameters.
        // This will result us in creating a parameter representation for each route parameter that does not
        // have a mapping parameter or bound property.
        ProcessRouteParameters(context);

        // Set IsRequired=true
        ProcessIsRequired(context, _mvcOptions);

        // Set DefaultValue
        ProcessParameterDefaultValue(context);

        return context.Results;
    }

    private void ProcessRouteParameters(SidApiParameterContext context)
    {
        var routeParameters = new Dictionary<string, ApiParameterRouteInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var routeParameter in context.RouteParameters)
        {
            routeParameters.Add(routeParameter.Name!, CreateRouteInfo(routeParameter));
        }

        for (var i = context.Results.Count - 1; i >= 0; i--)
        {
            var parameter = context.Results[i];

            if (parameter.Source == BindingSource.Path ||
               parameter.Source == BindingSource.ModelBinding ||
               parameter.Source == BindingSource.Custom)
            {
                if (routeParameters.TryGetValue(parameter.Name, out var routeInfo))
                {
                    parameter.RouteInfo = routeInfo;
                    routeParameters.Remove(parameter.Name);

                    if (parameter.Source == BindingSource.ModelBinding &&
                        !parameter.RouteInfo.IsOptional)
                    {
                        // If we didn't see any information about the parameter, but we have
                        // a route parameter that matches, let's switch it to path.
                        parameter.Source = BindingSource.Path;
                    }
                }
                else
                {
                    if (parameter.Source == BindingSource.Path &&
                        parameter.ModelMetadata is DefaultModelMetadata defaultModelMetadata &&
                        !defaultModelMetadata.Attributes.Attributes.OfType<IFromRouteMetadata>().Any())
                    {
                        // If we didn't see the parameter in the route and no FromRoute metadata is set, it probably means
                        // the parameter binding source was inferred (InferParameterBindingInfoConvention)  
                        // probably because another route to this action contains it as route parameter and
                        // will be removed from the API description
                        // https://github.com/dotnet/aspnetcore/issues/26234
                        context.Results.RemoveAt(i);
                    }
                }
            }
        }

        // Lastly, create a parameter representation for each route parameter that did not find
        // a partner.
        foreach (var routeParameter in routeParameters)
        {
            context.Results.Add(new ApiParameterDescription()
            {
                Name = routeParameter.Key,
                RouteInfo = routeParameter.Value,
                Source = BindingSource.Path,
            });
        }
    }

    private static void ProcessParameterDefaultValue(SidApiParameterContext context)
    {
        foreach (var parameter in context.Results)
        {
            if (parameter.Source == BindingSource.Path)
            {
                parameter.DefaultValue = parameter.RouteInfo?.DefaultValue;
            }
            else
            {
                if (parameter.ParameterDescriptor is ControllerParameterDescriptor controllerParameter &&
                    TryGetDeclaredParameterDefaultValue(controllerParameter.ParameterInfo, out var defaultValue))
                {
                    parameter.DefaultValue = defaultValue;
                }
            }
        }
    }

    private static bool TryGetDeclaredParameterDefaultValue(ParameterInfo parameterInfo, out object? defaultValue)
    {
        if (TryGetDefaultValue(parameterInfo, out defaultValue))
        {
            return true;
        }

        var defaultValueAttribute = parameterInfo.GetCustomAttribute<DefaultValueAttribute>(inherit: false);
        if (defaultValueAttribute != null)
        {
            defaultValue = defaultValueAttribute.Value;
            return true;
        }

        return false;
    }

    private static bool TryGetDefaultValue(ParameterInfo parameter, out object? defaultValue)
    {
        var hasDefaultValue = CheckHasDefaultValue(parameter, out var tryToGetDefaultValue);
        defaultValue = null;

        if (parameter.HasDefaultValue)
        {
            if (tryToGetDefaultValue)
            {
                defaultValue = parameter.DefaultValue;
            }

            bool isNullableParameterType = parameter.ParameterType.IsGenericType &&
                parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

            // Workaround for https://github.com/dotnet/runtime/issues/18599
            if (defaultValue == null && parameter.ParameterType.IsValueType
                && !isNullableParameterType) // Nullable types should be left null
            {
                defaultValue = CreateValueType(parameter.ParameterType);
            }

            // Handle nullable enums
            if (defaultValue != null && isNullableParameterType)
            {
                Type? underlyingType = Nullable.GetUnderlyingType(parameter.ParameterType);
                if (underlyingType != null && underlyingType.IsEnum)
                {
                    defaultValue = Enum.ToObject(underlyingType, defaultValue);
                }
            }
        }

        return hasDefaultValue;
    }

    private static bool CheckHasDefaultValue(ParameterInfo parameter, out bool tryToGetDefaultValue)
    {
        tryToGetDefaultValue = true;
        try
        {
            return parameter.HasDefaultValue;
        }
        catch (FormatException) when (parameter.ParameterType == typeof(DateTime))
        {
            // Workaround for https://github.com/dotnet/runtime/issues/18844
            // If HasDefaultValue throws FormatException for DateTime
            // we expect it to have default value
            tryToGetDefaultValue = false;
            return true;
        }
    }

    private static object? CreateValueType(Type t) => FormatterServices.GetSafeUninitializedObject(t);

    private ApiParameterRouteInfo CreateRouteInfo(TemplatePart routeParameter)
    {
        var constraints = new List<IRouteConstraint>();
        if (routeParameter.InlineConstraints != null)
        {
            foreach (var constraint in routeParameter.InlineConstraints)
            {
                constraints.Add(_constraintResolver.ResolveConstraint(constraint.Constraint)!);
            }
        }

        return new ApiParameterRouteInfo()
        {
            Constraints = constraints,
            DefaultValue = routeParameter.DefaultValue,
            IsOptional = routeParameter.IsOptional || routeParameter.DefaultValue != null,
        };
    }

    private static IEnumerable<string?> GetHttpMethods(ControllerActionDescriptor action)
    {
        if (action.ActionConstraints != null && action.ActionConstraints.Count > 0)
        {
            return action.ActionConstraints.OfType<HttpMethodActionConstraint>().SelectMany(c => c.HttpMethods);
        }
        else
        {
            return new string?[] { null };
        }
    }

    private static IApiRequestMetadataProvider[]? GetRequestMetadataAttributes(ControllerActionDescriptor action)
    {
        if (action.FilterDescriptors == null)
        {
            return null;
        }

        return action.FilterDescriptors
            .Select(fd => fd.Filter)
            .OfType<IApiRequestMetadataProvider>()
            .ToArray();
    }

    private static void ProcessIsRequired(SidApiParameterContext context, MvcOptions mvcOptions)
    {
        foreach (var parameter in context.Results)
        {
            if (parameter.Source == BindingSource.Body)
            {
                if (parameter.BindingInfo == null || parameter.BindingInfo.EmptyBodyBehavior == EmptyBodyBehavior.Default)
                {
                    parameter.IsRequired = !mvcOptions.AllowEmptyInputInBodyModelBinding;
                }
                else
                {
                    parameter.IsRequired = !(parameter.BindingInfo.EmptyBodyBehavior == EmptyBodyBehavior.Allow);
                }
            }

            if (parameter.ModelMetadata != null && parameter.ModelMetadata.IsBindingRequired)
            {
                parameter.IsRequired = true;
            }

            if (parameter.Source == BindingSource.Path && parameter.RouteInfo != null && !parameter.RouteInfo.IsOptional)
            {
                parameter.IsRequired = true;
            }
        }
    }

    private static MediaTypeCollection GetDeclaredContentTypes(IReadOnlyList<IApiRequestMetadataProvider>? requestMetadataAttributes, IAcceptsMetadata? acceptsMetadata)
    {
        var contentTypes = new MediaTypeCollection();

        // Walking the content types from the accepts metadata first
        // to allow any RequestMetadataProvider to see or override any accepts metadata
        // keeping the current behavior.
        if (acceptsMetadata != null)
        {
            foreach (var contentType in acceptsMetadata.ContentTypes)
            {
                contentTypes.Add(contentType);
            }
        }

        // Walk through all 'filter' attributes in order, and allow each one to see or override
        // the results of the previous ones. This is similar to the execution path for content-negotiation.
        if (requestMetadataAttributes != null)
        {
            foreach (var metadataAttribute in requestMetadataAttributes)
            {
                metadataAttribute.SetContentTypes(contentTypes);
            }
        }

        return contentTypes;
    }

    private static RouteTemplate? ParseTemplate(SidConventionalRouteEntry route)
    {
        if (route.Pattern != null)
        {
            return TemplateParser.Parse(route.Pattern.RawText);
        }

        return null;
    }

    private string? GetRelativePath(RouteTemplate? parsedTemplate)
    {
        if (parsedTemplate == null)
        {
            return null;
        }

        var segments = new List<string>();
        foreach (var segment in parsedTemplate.Segments)
        {
            var currentSegment = string.Empty;
            foreach (var part in segment.Parts)
            {
                if (part.IsLiteral)
                {
                    currentSegment += _routeOptions.LowercaseUrls ?
                        part.Text!.ToLowerInvariant() :
                        part.Text;
                }
                else if (part.IsParameter)
                {
                    currentSegment += "{" + part.Name + "}";
                }
            }

            segments.Add(currentSegment);
        }

        return string.Join("/", segments);
    }

    private sealed class PseudoModelBindingVisitor
    {
        public PseudoModelBindingVisitor(SidApiParameterContext context, ParameterDescriptor parameter)
        {
            Context = context;
            Parameter = parameter;

            Visited = new HashSet<PropertyKey>(new PropertyKeyEqualityComparer());
        }

        public SidApiParameterContext Context { get; }

        public ParameterDescriptor Parameter { get; }

        // Avoid infinite recursion by tracking properties.
        private HashSet<PropertyKey> Visited { get; }

        public void WalkParameter(ApiParameterDescriptionContext context)
        {
            // Attempt to find a binding source for the parameter
            //
            // The default is ModelBinding (aka all default value providers)
            var source = BindingSource.ModelBinding;
            Visit(context, source, containerName: string.Empty);
        }

        private void Visit(
            ApiParameterDescriptionContext bindingContext,
            BindingSource ambientSource,
            string containerName)
        {
            var source = bindingContext.BindingSource;
            if (source != null && source.IsGreedy)
            {
                // We have a definite answer for this model. This is a greedy source like
                // [FromBody] so there's no need to consider properties.
                Context.Results.Add(CreateResult(bindingContext, source, containerName));
                return;
            }

            var modelMetadata = bindingContext.ModelMetadata;

            // For any property which is a leaf node, we don't want to keep traversing:
            //
            //  1)  Collections - while it's possible to have binder attributes on the inside of a collection,
            //      it hardly seems useful, and would result in some very weird binding.
            //
            //  2)  Simple Types - These are generally part of the .net framework - primitives, or types which have a
            //      type converter from string.
            //
            //  3)  Types with no properties. Obviously nothing to explore there.
            //
            if (modelMetadata.IsEnumerableType ||
                !modelMetadata.IsComplexType ||
                modelMetadata.Properties.Count == 0)
            {
                Context.Results.Add(CreateResult(bindingContext, source ?? ambientSource, containerName));
                return;
            }

            // This will come from composite model binding - so investigate what's going on with each property.
            //
            // Ex:
            //
            //      public IActionResult PlaceOrder(OrderDTO order) {...}
            //
            //      public class OrderDTO
            //      {
            //          public int AccountId { get; set; }
            //
            //          [FromBody]
            //          public Order { get; set; }
            //      }
            //
            // This should result in two parameters:
            //
            //  AccountId - source: Any
            //  Order - source: Body
            //

            // We don't want to append the **parameter** name when building a model name.
            var newContainerName = containerName;
            if (modelMetadata.ContainerType != null)
            {
                newContainerName = GetName(containerName, bindingContext);
            }

            var metadataProperties = modelMetadata.Properties;
            var metadataPropertiesCount = metadataProperties.Count;
            for (var i = 0; i < metadataPropertiesCount; i++)
            {
                var propertyMetadata = metadataProperties[i];
                var key = new PropertyKey(propertyMetadata, source);
                var bindingInfo = BindingInfo.GetBindingInfo(Enumerable.Empty<object>(), propertyMetadata);

                var propertyContext = new ApiParameterDescriptionContext(
                    propertyMetadata,
                    bindingInfo: bindingInfo,
                    propertyName: null);

                if (Visited.Add(key))
                {
                    Visit(propertyContext, source ?? ambientSource, newContainerName);
                    Visited.Remove(key);
                }
                else
                {
                    // This is cycle, so just add a result rather than traversing.
                    Context.Results.Add(CreateResult(propertyContext, source ?? ambientSource, newContainerName));
                }
            }
        }

        private ApiParameterDescription CreateResult(
            ApiParameterDescriptionContext bindingContext,
            BindingSource source,
            string containerName)
        {
            return new ApiParameterDescription()
            {
                ModelMetadata = bindingContext.ModelMetadata,
                Name = GetName(containerName, bindingContext),
                Source = source,
                Type = GetModelType(bindingContext.ModelMetadata),
                ParameterDescriptor = Parameter,
                BindingInfo = bindingContext.BindingInfo
            };
        }

        private static Type GetModelType(ModelMetadata metadata)
        {
            // IsParseableType || IsConvertibleType
            if (!metadata.IsComplexType)
            {
                return GetDisplayType(metadata.ModelType);
            }

            return metadata.ModelType;
        }

        private static string GetName(string containerName, ApiParameterDescriptionContext metadata)
        {
            var propertyName = !string.IsNullOrEmpty(metadata.BinderModelName) ? metadata.BinderModelName : metadata.PropertyName;
            return ModelNames.CreatePropertyModelName(containerName, propertyName);
        }

        private readonly struct PropertyKey
        {
            public readonly Type ContainerType;

            public readonly string PropertyName;

            public readonly BindingSource? Source;

            public PropertyKey(ModelMetadata metadata, BindingSource? source)
            {
                ContainerType = metadata.ContainerType!;
                PropertyName = metadata.PropertyName!;
                Source = source;
            }
        }

        private sealed class PropertyKeyEqualityComparer : IEqualityComparer<PropertyKey>
        {
            public bool Equals(PropertyKey x, PropertyKey y)
            {
                return
                    x.ContainerType == y.ContainerType &&
                    x.PropertyName == y.PropertyName &&
                    x.Source == y.Source;
            }

            public int GetHashCode(PropertyKey obj)
            {
                return HashCode.Combine(obj.ContainerType, obj.PropertyName, obj.Source);
            }
        }
    }

    private sealed class ApiParameterDescriptionContext
    {
        public ModelMetadata ModelMetadata { get; }

        public string? BinderModelName { get; }

        public BindingSource? BindingSource { get; }

        public string? PropertyName { get; }

        public BindingInfo? BindingInfo { get; }

        public ApiParameterDescriptionContext(
            ModelMetadata metadata,
            BindingInfo? bindingInfo,
            string? propertyName)
        {
            // BindingMetadata can be null if the metadata represents properties.
            ModelMetadata = metadata;
            BinderModelName = bindingInfo?.BinderModelName;
            BindingSource = bindingInfo?.BindingSource;
            PropertyName = propertyName ?? metadata.Name;
            BindingInfo = bindingInfo;
        }
    }

    private static Type GetDisplayType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        return underlyingType.IsPrimitive
            // Those additional types have TypeConverter or TryParse and are not primitives
            // but should not be considered string in the metadata
            || underlyingType == typeof(DateTime)
            || underlyingType == typeof(DateTimeOffset)
            || underlyingType == typeof(DateOnly)
            || underlyingType == typeof(TimeOnly)
            || underlyingType == typeof(TimeSpan)
            || underlyingType == typeof(decimal)
            || underlyingType == typeof(Guid)
            || underlyingType == typeof(Uri) ? type : typeof(string);
    }
}
