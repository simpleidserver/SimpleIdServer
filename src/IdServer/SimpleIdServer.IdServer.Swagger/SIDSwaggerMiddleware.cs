// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using SimpleIdServer.IdServer.Options;
using Swashbuckle.AspNetCore.Swagger;
using System.Globalization;
using System.Text;

namespace SimpleIdServer.IdServer.Swagger;

public class SIDSwaggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SwaggerOptions _options;
    private readonly IdServerHostOptions _idOptions;

    public SIDSwaggerMiddleware(
        RequestDelegate next,
        SwaggerOptions options,
        IOptions<IdServerHostOptions> idOptions)
    {
        _next = next;
        _options = options ?? new SwaggerOptions();
        _idOptions = idOptions.Value;
    }

    public async Task Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
    {
        string realm = null;
        if (!RequestingSwaggerDocument(httpContext.Request, out string documentName, out realm))
        {
            await _next(httpContext);
            return;
        }

        try
        {
            var basePath = httpContext.Request.PathBase.HasValue
                ? httpContext.Request.PathBase.Value
                : null;

            var swagger = swaggerProvider switch
            {
                IAsyncSwaggerProvider asyncSwaggerProvider => await asyncSwaggerProvider.GetSwaggerAsync(
                    documentName: documentName,
                    host: null,
                    basePath: basePath),
                _ => swaggerProvider.GetSwagger(
                    documentName: documentName,
                    host: null,
                    basePath: basePath)
            };

            swagger.Paths = Transform(swagger.Paths, realm);
            swagger.Components.SecuritySchemes = Transform(swagger.Components.SecuritySchemes, realm);

            foreach (var filter in _options.PreSerializeFilters)
            {
                filter(swagger, httpContext.Request);
            }

            if (Path.GetExtension(httpContext.Request.Path.Value) == ".yaml")
            {
                await RespondWithSwaggerYaml(httpContext.Response, swagger);
            }
            else
            {
                await RespondWithSwaggerJson(httpContext.Response, swagger);
            }
        }
        catch (UnknownSwaggerDocument)
        {
            RespondWithNotFound(httpContext.Response);
        }
    }

    private OpenApiPaths Transform(OpenApiPaths paths, string realm)
    {
        if (!_idOptions.RealmEnabled) return paths;
        var result = new OpenApiPaths();
        foreach (var path in paths)
        {
            var key = path.Key.Replace("{" + Constants.Prefix + "}", realm);
            result.Add(key, path.Value);
        }

        return result;
    }

    private IDictionary<string, OpenApiSecurityScheme> Transform(IDictionary<string, OpenApiSecurityScheme> securitySchemes, string realm)
    {
        var baseUrl = _idOptions.Authority;
        if (_idOptions.RealmEnabled) baseUrl = $"{baseUrl}/{realm}";
        foreach(var kvp in securitySchemes)
        {
            kvp.Value.Flows.AuthorizationCode.AuthorizationUrl = new Uri($"{baseUrl}/authorization");
            kvp.Value.Flows.AuthorizationCode.TokenUrl = new Uri($"{baseUrl}/token");
        }

        return securitySchemes;
    }

    private bool RequestingSwaggerDocument(HttpRequest request, out string documentName, out string realm)
    {
        realm = null;
        var routeTemplate = _options.RouteTemplate.TrimStart('/');
        if (_idOptions.RealmEnabled) routeTemplate = "{"+ Constants.Prefix + "}/" + routeTemplate;
        var requestMatcher = new TemplateMatcher(TemplateParser.Parse(routeTemplate), new RouteValueDictionary());
        documentName = null;
        if (request.Method != "GET") return false;

        var routeValues = new RouteValueDictionary();
        if (!requestMatcher.TryMatch(request.Path, routeValues) || !routeValues.ContainsKey("documentName")) return false;

        documentName = routeValues["documentName"].ToString();
        if (_idOptions.RealmEnabled) realm = routeValues[Constants.Prefix].ToString();
        return true;
    }

    private void RespondWithNotFound(HttpResponse response)
    {
        response.StatusCode = 404;
    }

    private async Task RespondWithSwaggerJson(HttpResponse response, OpenApiDocument swagger)
    {
        response.StatusCode = 200;
        response.ContentType = "application/json;charset=utf-8";

        using (var textWriter = new StringWriter(CultureInfo.InvariantCulture))
        {
            var jsonWriter = new OpenApiJsonWriter(textWriter);
            if (_options.SerializeAsV2) swagger.SerializeAsV2(jsonWriter); else swagger.SerializeAsV3(jsonWriter);

            await response.WriteAsync(textWriter.ToString(), new UTF8Encoding(false));
        }
    }

    private async Task RespondWithSwaggerYaml(HttpResponse response, OpenApiDocument swagger)
    {
        response.StatusCode = 200;
        response.ContentType = "text/yaml;charset=utf-8";

        using (var textWriter = new StringWriter(CultureInfo.InvariantCulture))
        {
            var yamlWriter = new OpenApiYamlWriter(textWriter);
            if (_options.SerializeAsV2) swagger.SerializeAsV2(yamlWriter); else swagger.SerializeAsV3(yamlWriter);

            await response.WriteAsync(textWriter.ToString(), new UTF8Encoding(false));
        }
    }
}
