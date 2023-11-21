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
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Infastructures;

namespace SimpleIdServer.IdServer.Swagger;

public class SidApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
{
    private const string _defaultVersion = "v1";
    private readonly ISidEndpointStore _edpsStore;
    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
    private readonly MvcOptions _mvcOptions;
    private readonly RouteOptions _routeOptions;
    private ApiDescriptionGroupCollection? _apiDescriptionGroups;

    public SidApiDescriptionGroupCollectionProvider(
        ISidEndpointStore edpsStore,
        IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
        IOptions<MvcOptions> optionsAccessor,
        IOptions<RouteOptions> routeOptions)
    {

        _edpsStore = edpsStore;
        _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
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
            var routeEndpoint = _edpsStore.Routes.SingleOrDefault(e =>
                e.Pattern.Defaults["controller"].ToString() == action.ControllerName &&
                e.Pattern.Defaults["action"].ToString() == action.ActionName);
            if (routeEndpoint == null) continue;

            var httpMethods = GetHttpMethods(action);
            foreach (var httpMethod in httpMethods)
            {
                var record = CreateApiDescription(action, httpMethod, _defaultVersion, routeEndpoint);
                if (!result.Any(r => r.ActionDescriptor.Id == record.ActionDescriptor.Id))
                    result.Add(record);
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
        var parsedTemplate = ParseTemplate(route);
        var apiDescription = new ApiDescription()
        {
            ActionDescriptor = action,
            GroupName = groupName,
            HttpMethod = httpMethod,
            RelativePath = GetRelativePath(parsedTemplate),
        };

        var templateParameters = parsedTemplate?.Parameters?.ToList() ?? new List<TemplatePart>();

        /*
        var parameterContext = new ApiParameterContext(_modelMetadataProvider, action, templateParameters);

        foreach (var parameter in GetParameters(parameterContext))
        {
            apiDescription.ParameterDescriptions.Add(parameter);
        }

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
}
