// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace FormBuilder.Helpers;

public interface IHttpRequestState
{
    string Scheme { get; }
    string Host { get; }
    string PathBase { get; }
    bool IsInitialized { get; }
    void Initialize(string scheme, string host, string pathBase);
    string GetAbsoluteUriWithVirtualPath();
}

public class HttpRequestState : IHttpRequestState
{
    public string Scheme { get; private set; }
    public string Host { get; private set; }
    public string PathBase { get; private set; }
    public bool IsInitialized { get; private set; }

    public void Initialize(string scheme, string host, string pathBase)
    {
        Scheme = scheme;
        Host = host;
        PathBase = pathBase;
        IsInitialized = true;
    }

    public string GetAbsoluteUriWithVirtualPath()
    {
        if (!IsInitialized) return string.Empty;
        return $"{Scheme}://{Host}{PathBase}";
    }
}
