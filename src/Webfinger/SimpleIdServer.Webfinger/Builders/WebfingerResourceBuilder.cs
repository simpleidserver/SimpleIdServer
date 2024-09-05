// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Webfinger.Models;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Webfinger.Builders;

public class WebfingerResourceBuilder
{
    private string _scheme;
    private string _subject;
    private readonly List<WebfingerResource> _resources = new List<WebfingerResource>();

    private WebfingerResourceBuilder(string scheme, string subject)
    {
        _scheme = scheme;
        _subject = subject;
    }

    // REL = https://openid.net/specs/fastfed/1.0/provider
    public static WebfingerResourceBuilder New(string scheme, string subject)
    {
        return new WebfingerResourceBuilder(scheme, subject);
    }

    public WebfingerResourceBuilder AddLinkRelation(string rel, string href)
    {
        var resource = new WebfingerResource
        {
            Id = Guid.NewGuid().ToString(),
            Subject = _subject,
            Scheme = _scheme,
            Href = href,
            Rel = rel,
        };
        _resources.Add(resource);
        return this;
    }

    public List<WebfingerResource> Build() => _resources;
}