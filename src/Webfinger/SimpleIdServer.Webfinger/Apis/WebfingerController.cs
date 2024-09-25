// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Webfinger.Client;
using SimpleIdServer.Webfinger.Stores;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Webfinger.Apis;

public class WebfingerController : Controller
{
    private readonly IWebfingerResourceStore _resourceStore;

    public WebfingerController(IWebfingerResourceStore resourceStore)
    {
        _resourceStore = resourceStore;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetWebfingerRequest request, CancellationToken cancellationToken)
    {
        var splittedResource = request.Resource.Split(':');
        var scheme = splittedResource.First();
        var subject = string.Join(":", splittedResource.Skip(1));
        var webfingerResources = await _resourceStore.GetResources(scheme, subject, request.Rel, cancellationToken);
        var result = new GetWebfingerResult
        {
            Subject = subject,
            Links = webfingerResources.Select(r => new WebfingerLinkResult
            {
                Href = r.Href,
                Rel = r.Rel
            }).ToList()
        };
        return new ContentResult
        {
            Content = JsonSerializer.Serialize(result),
            ContentType = "application/jrd+json"
        };
    }
}
