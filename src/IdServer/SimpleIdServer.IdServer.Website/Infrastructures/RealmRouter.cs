// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace SimpleIdServer.IdServer.Website.Infrastructures;

public class RealmRouter : IComponent, IHandleAfterRender, IDisposable
{
    string _baseUri;
    string _locationAbsolute;
    bool _navigationInterceptionEnabled;
    private RenderHandle _renderHandle;
    private Dictionary<Type, Dictionary<string, string>> _routeableComponents;
    internal static IServiceProvider _serviceProvider;
    private CancellationTokenSource _onNavigateCts;
    private Task _previousOnNavigateTask = Task.CompletedTask;
    private IRoutingStateProvider? RoutingStateProvider { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] IServiceProvider ServiceProvider { get; set; }
    [Inject] private IScrollToLocationHash ScrollToLocationHash { get; set; }
    [Inject] private INavigationInterception NavigationInterception { get; set; }
    [Parameter] public Assembly AppAssembly { get; set; }
    [Parameter] public RenderFragment<RouteData> Found { get; set; }
    [Parameter] public RenderFragment NotFound { get; set; }
    [Parameter] public RenderFragment? Navigating { get; set; }
    [Parameter] public EventCallback<CustomNavigationContext> OnNavigateAsync { get; set; }
    private bool _onNavigateCalled;

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
        _routeableComponents = GetRouteableComponents();
        _baseUri = NavigationManager.BaseUri;
        _locationAbsolute = NavigationManager.Uri;
        NavigationManager.LocationChanged += OnLocationChanged;
        RoutingStateProvider = ServiceProvider.GetService<IRoutingStateProvider>();
    }

    public async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        var locationPath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        if (!_onNavigateCalled)
        {
            _onNavigateCalled = true;
            await RunOnNavigateAsync(locationPath, false);
        }
        else
        {
            await Refresh(locationPath, false);
        }
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }

    public async Task OnAfterRenderAsync()
    {
        if (!_navigationInterceptionEnabled)
        {
            _navigationInterceptionEnabled = true;
            await NavigationInterception.EnableNavigationInterceptionAsync();
        }
    }

    private async Task RunOnNavigateAsync(string locationPath, bool isNavigationIntercepted)
    {
        _onNavigateCts?.Cancel();
        await _previousOnNavigateTask;
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _previousOnNavigateTask = tcs.Task;
        if (!OnNavigateAsync.HasDelegate)
        {
            await Refresh(locationPath, isNavigationIntercepted);
        }

        _onNavigateCts = new CancellationTokenSource();
        var navigateContext = new CustomNavigationContext(locationPath,  _onNavigateCts.Token);
        var cancellationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        navigateContext.CancellationToken.Register(state =>
            ((TaskCompletionSource)state).SetResult(), cancellationTcs);

        try
        {
            var task = await Task.WhenAny(OnNavigateAsync.InvokeAsync(navigateContext), cancellationTcs.Task);
            await task;
            tcs.SetResult();
            await Refresh(locationPath, isNavigationIntercepted);
        }
        catch (Exception e)
        {
            _renderHandle.Render(builder => ExceptionDispatchInfo.Throw(e));
        }
    }

    private async Task Refresh(string locationPath, bool isNavigationIntercepted)
    {
        if (_previousOnNavigateTask.Status != TaskStatus.RanToCompletion)
        {
            if (Navigating != null)
            {
                _renderHandle.Render(Navigating);
            }
            return;
        }


        var options = _serviceProvider.GetRequiredService<IOptions<IdServerWebsiteOptions>>();
        var routeParameters = new Dictionary<string, object>();
        Type handlerContext = null;
        var relativePath = NavigationManager.ToBaseRelativePath(_locationAbsolute);
        if (!options.Value.IsReamEnabled)
        {
            if (!locationPath.StartsWith("/"))
                locationPath = $"/{locationPath}";
            if (!IsMatch(locationPath, _routeableComponents, out routeParameters, out handlerContext))
            {
                _renderHandle.Render(NotFound);
                return;
            }

            var routeData = new RouteData(handlerContext, routeParameters);
            _renderHandle.Render(Found(routeData)); 
            return;
        }
        else
        {
            var realms = await GetRealms(options);
            var realm = string.IsNullOrWhiteSpace(locationPath) ? Constants.DefaultRealm : locationPath.Split("/").First();
            var pathWithoutRealm = locationPath.Replace(realm, string.Empty);
            if (!IsMatch(pathWithoutRealm, _routeableComponents, out routeParameters, out handlerContext))
            {
                _renderHandle.Render(NotFound);
                return;
            }

            var routeData = new RouteData(handlerContext, routeParameters);
            _renderHandle.Render(Found(routeData));
        }
    }

    private async void OnLocationChanged(object sender, LocationChangedEventArgs args)
    {
        if(_renderHandle.IsInitialized && _routeableComponents != null)
        {
            var locationPath = NavigationManager.ToBaseRelativePath(args.Location);
            _ = RunOnNavigateAsync((locationPath), args.IsNavigationIntercepted);
        }
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

    private bool IsMatch(string path, Dictionary<Type, Dictionary<string, string>> routableComponents, out Dictionary<string, object> parameters, out Type handlerContext)
    {
        parameters = new Dictionary<string, object>();
        handlerContext = null;
        var context = routableComponents.FirstOrDefault(c => c.Value.Any(r => Regex.IsMatch(path, r.Value)));
        if (context.Key == null) return false;
        handlerContext = context.Key;
        var kvp = context.Value.First(c => Regex.IsMatch(path, c.Value));
        var templateElements = kvp.Key.Split("/");
        var urlElements = path.Split("/");
        for(var i = 0; i < templateElements.Count(); i++)
        {
            var elt = templateElements[i];
            if (Regex.IsMatch(elt, "^{\\w*}$"))
                parameters.Add(elt.Replace("{", "").Replace("}", ""), urlElements.ElementAt(i));
        }

        return true;
    }

    private Dictionary<Type, Dictionary<string, string>> GetRouteableComponents()
    {
        var assembly = typeof(RealmRouter).Assembly;
        var components = new List<Type>();
        var dic = new Dictionary<Type, Dictionary<string, string>>();
        foreach(var type in assembly.ExportedTypes)
        {
            if(typeof(IComponent).IsAssignableFrom(type) && type.IsDefined(typeof(RouteAttribute)))
            {
                var routeAttributes = type.GetCustomAttributes<RouteAttribute>(inherit: false);
                var templates = routeAttributes.ToDictionary(r => r.Template, r => $"^{Regex.Replace(r.Template, "{\\w*}", "(\\d|\\w|-)*")}$");
                if(!templates.ContainsKey(@"/"))
                    dic.Add(type, templates);
            }
        }

        return dic;
    }
}

public class CustomNavigationContext
{
    public CustomNavigationContext(string path, CancellationToken cancellationToken)
    {
        Path = path;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// The target path for the navigation.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// The <see cref="CancellationToken"/> to use to cancel navigation.
    /// </summary>
    public CancellationToken CancellationToken { get; }

}