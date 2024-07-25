// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleIdServer.IdServer.Website.Infrastructures;

public class RealmRouter : IComponent, IHandleAfterRender, IDisposable
{
    private RenderHandle _renderHandle;
    internal static IServiceProvider _serviceProvider;
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Parameter] public Assembly AppAssembly { get; set; }
    [Parameter] public RenderFragment<RouteData> Found { get; set; }
    [Parameter] public RenderFragment NotFound { get; set; }

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
    }

    public async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        var options = _serviceProvider.GetRequiredService<IOptions<IdServerWebsiteOptions>>();
        var components = GetRouteableComponents();
        if (!options.Value.IsReamEnabled)
        {
            var locationPath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var context = components.FirstOrDefault(c => c.Value.Any(r => Regex.IsMatch(locationPath, r)));
            if(context.Key == null)
            {
                _renderHandle.Render(NotFound);
                return;
            }

            var routeData = new RouteData(context.Key, new Dictionary<string, object>());
            _renderHandle.Render(Found(routeData));
            return;
        }
        else
        {            
            var realms = await GetRealms(options);
            var locationPath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var realm = string.IsNullOrWhiteSpace(locationPath) ? Constants.DefaultRealm : locationPath.Split("/").First();
            _serviceProvider.GetRequiredService<CurrentRealm>().Identifier = realm;
            var pathWithoutRealm = locationPath.Replace(realm, string.Empty);
            var context = components.FirstOrDefault(c => c.Value.Any(r => Regex.IsMatch(pathWithoutRealm, r)));
            if (!realms.Any(r => r.Name == realm) || context.Key == null)
            {
                _renderHandle.Render(NotFound);
                return;
            }

            var routeData = new RouteData(context.Key, new Dictionary<string, object>());
            _renderHandle.Render(Found(routeData));
        }
    }

    public void Dispose()
    {
    }

    public Task OnAfterRenderAsync()
    {
        return Task.CompletedTask;
    }

    private async Task<IEnumerable<Realm>> GetRealms(IOptions<IdServerWebsiteOptions> options)
    {
        var httpClientFactory = _serviceProvider.GetRequiredService<IWebsiteHttpClientFactory>();
        var url = $"{options.Value.IdServerBaseUrl}/realms";
        var httpClient = await httpClientFactory.Build();
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var realms = SidJsonSerializer.Deserialize<IEnumerable<Realm>>(json);
        return realms;
    }

    private Dictionary<Type, List<string>> GetRouteableComponents()
    {
        var assembly = typeof(RealmRouter).Assembly;
        var components = new List<Type>();
        var dic = new Dictionary<Type, List<string>>();
        foreach(var type in assembly.ExportedTypes)
        {
            if(typeof(IComponent).IsAssignableFrom(type) && type.IsDefined(typeof(RouteAttribute)))
            {
                var routeAttributes = type.GetCustomAttributes<RouteAttribute>(inherit: false);
                var templates = routeAttributes.Select(r => Regex.Replace(r.Template, "{\\w*}", "\\w*")).ToList();
                dic.Add(type, templates);
            }
        }

        return dic;
    }
}
