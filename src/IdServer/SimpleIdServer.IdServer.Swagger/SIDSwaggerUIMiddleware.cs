// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SimpleIdServer.IdServer.Swagger;

public class SidSwaggerUIMiddleware
{
    private const string EmbeddedFileNamespace = "Swashbuckle.AspNetCore.SwaggerUI.node_modules.swagger_ui_dist";
    private readonly SwaggerUIOptions _options;
    private readonly StaticFileMiddleware _staticFileMiddleware;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IdServerHostOptions _idOptions;

    public SidSwaggerUIMiddleware(
        RequestDelegate next,
        IWebHostEnvironment hostingEnv,
        ILoggerFactory loggerFactory,
        SwaggerUIOptions options,
        IOptions<IdServerHostOptions> idOptions)
    {
        _options = options ?? new SwaggerUIOptions();
        _idOptions = idOptions.Value;

        _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options);

        _jsonSerializerOptions = new JsonSerializerOptions();
#if NET6_0
            _jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
#else
        _jsonSerializerOptions.IgnoreNullValues = true;
#endif
        _jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var httpMethod = httpContext.Request.Method;
        var path = httpContext.Request.Path.Value;

        // If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
        if (httpMethod == "GET" && Regex.IsMatch(path, $"{GetRegex()}$", RegexOptions.IgnoreCase))
        {
            // Use relative redirect to support proxy environments
            var relativeIndexUrl = string.IsNullOrEmpty(path) || path.EndsWith("/")
                ? "index.html"
                : $"{path}/index.html";

            RespondWithRedirect(httpContext.Response, relativeIndexUrl);
            return;
        }

        if (httpMethod == "GET" && Regex.IsMatch(path, $"^{GetRegex()}index.html$", RegexOptions.IgnoreCase))
        {
            await RespondWithIndexHtml(httpContext.Response);
            return;
        }

        await _staticFileMiddleware.Invoke(httpContext);
    }

    private StaticFileMiddleware CreateStaticFileMiddleware(
        RequestDelegate next,
        IWebHostEnvironment hostingEnv,
        ILoggerFactory loggerFactory,
        SwaggerUIOptions options)
    {
        var staticFileOptions = new StaticFileOptions
        {
            RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
            FileProvider = new EmbeddedFileProvider(typeof(SwaggerUIMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace),
        };


        return new StaticFileMiddleware(next, hostingEnv, Microsoft.Extensions.Options.Options.Create(staticFileOptions), loggerFactory);
    }

    private void RespondWithRedirect(HttpResponse response, string location)
    {
        response.StatusCode = 301;
        response.Headers["Location"] = location;
    }

    private async Task RespondWithIndexHtml(HttpResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = "text/html;charset=utf-8";

        using (var stream = _options.IndexStream())
        {
            using var reader = new StreamReader(stream);

            // Inject arguments before writing to response
            var htmlBuilder = new StringBuilder(await reader.ReadToEndAsync());
            foreach (var entry in GetIndexArguments())
            {
                htmlBuilder.Replace(entry.Key, entry.Value);
            }

            await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        }
    }

    private string GetRegex()
    {
        if (_idOptions.UseRealm) return @$"^\/(.)*\/{_options.RoutePrefix}\/?";
        return @$"^\/{_options.RoutePrefix}\/?";
    }

    private IDictionary<string, string> GetIndexArguments()
    {
        return new Dictionary<string, string>()
        {
            { "%(DocumentTitle)", _options.DocumentTitle },
            { "%(HeadContent)", FormatHeadContent() },
            { "%(ConfigObject)", JsonSerializer.Serialize(_options.ConfigObject, _jsonSerializerOptions) },
            { "%(OAuthConfigObject)", JsonSerializer.Serialize(_options.OAuthConfigObject, _jsonSerializerOptions) },
            { "%(Interceptors)", JsonSerializer.Serialize(_options.Interceptors) },
        };
    }

    private string FormatHeadContent()
    {
        if (!_idOptions.UseRealm) return _options.HeadContent;
        var strBuilder = new StringBuilder(_options.HeadContent);
        strBuilder.AppendLine($"<link href='../../{_options.RoutePrefix}/swagger-ui.css' rel='stylesheet' media='screen' type='text/css' />");
        strBuilder.AppendLine($"<script src='../../{_options.RoutePrefix}/swagger-ui-bundle.js'></script>");
        strBuilder.AppendLine($"<script src='../../{_options.RoutePrefix}/swagger-ui-standalone-preset.js'></script>");
        return strBuilder.ToString();
    }
}