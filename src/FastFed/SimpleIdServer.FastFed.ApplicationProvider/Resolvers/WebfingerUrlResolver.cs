// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.FastFed.ApplicationProvider.Resolvers;

public interface IWebfingerUrlResolver
{
    string Resolve(string domainName);
}

public class WebfingerUrlResolver : IWebfingerUrlResolver
{
    public string Resolve(string domainName)
        => $"https://{domainName}/{Webfinger.Client.RouteNames.WellKnownWebFinger}";
}
