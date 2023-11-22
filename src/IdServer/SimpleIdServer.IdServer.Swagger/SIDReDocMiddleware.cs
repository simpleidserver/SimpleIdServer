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
using Swashbuckle.AspNetCore.ReDoc;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SimpleIdServer.IdServer.Swagger;

public class SIDReDocMiddleware
{
    private const string EmbeddedFileNamespace = "Swashbuckle.AspNetCore.ReDoc.node_modules.redoc.bundles";
    private readonly ReDocOptions _options;
    private readonly IdServerHostOptions _idOptions;
    private readonly StaticFileMiddleware _staticFileMiddleware;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SIDReDocMiddleware(
        RequestDelegate next,
        IOptions<IdServerHostOptions> idOptions,
        IWebHostEnvironment hostingEnv,
        ILoggerFactory loggerFactory,
        ReDocOptions options)
    {
        _options = options ?? new ReDocOptions();
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
                : $"{path.Split('/').Last()}/index.html";

            RespondWithRedirect(httpContext.Response, relativeIndexUrl);
            return;
        }

        if (httpMethod == "GET" && Regex.IsMatch(path, $"{GetRegex()}index.html$", RegexOptions.IgnoreCase))
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
        ReDocOptions options)
    {
        var staticFileOptions = new StaticFileOptions
        {
            RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
            FileProvider = new EmbeddedFileProvider(typeof(ReDocMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace),
        };

        return new StaticFileMiddleware(next, hostingEnv, Microsoft.Extensions.Options.Options.Create(staticFileOptions), loggerFactory);
    }

    private string GetRegex()
    {
        if (_idOptions.UseRealm) return @$"^\/(.)*\/{_options.RoutePrefix}\/?";
        return @$"^\/{_options.RoutePrefix}\/?";
    }

    private void RespondWithRedirect(HttpResponse response, string location)
    {
        response.StatusCode = 301;
        response.Headers["Location"] = location;
    }

    private async Task RespondWithIndexHtml(HttpResponse response)
    {
        response.StatusCode = 200;
        response.ContentType = "text/html";

        using (var stream = _options.IndexStream())
        {
            // Inject arguments before writing to response
            var htmlBuilder = new StringBuilder(new StreamReader(stream).ReadToEnd());
            foreach (var entry in GetIndexArguments())
            {
                htmlBuilder.Replace(entry.Key, entry.Value);
            }

            await response.WriteAsync(htmlBuilder.ToString(), Encoding.UTF8);
        }
    }

    private IDictionary<string, string> GetIndexArguments()
    {
        return new Dictionary<string, string>()
        {
            { "%(DocumentTitle)", _options.DocumentTitle },
            { "%(HeadContent)", FormatHeadContent() },
            { "%(SpecUrl)", _options.SpecUrl },
            { "%(ConfigObject)", JsonSerializer.Serialize(_options.ConfigObject, _jsonSerializerOptions) }
        };
    }

    private string FormatHeadContent()
    {
        if (!_idOptions.UseRealm) return _options.HeadContent;
        var strBuilder = new StringBuilder(_options.HeadContent);
        strBuilder.AppendLine($"<script src='../../{_options.RoutePrefix}/redoc.standalone.js'></script>");
        return strBuilder.ToString();
    }
}
