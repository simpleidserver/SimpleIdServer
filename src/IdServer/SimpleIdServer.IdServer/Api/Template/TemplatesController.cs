// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Template;

public class TemplatesController : BaseController
{
    private readonly ITemplateStore _templateStore;
    private readonly ILogger<TemplatesController> _logger;

    public TemplatesController(ITokenRepository tokenRepository, IJwtBuilder jwtBuilder, ITemplateStore templateStore, ILogger<TemplatesController> logger) : base(tokenRepository, jwtBuilder)
    {
        _templateStore = templateStore;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var template = await _templateStore.GetActive(prefix, cancellationToken);
        if (template == null)
        {
            return new NoContentResult();
        }

        return new OkObjectResult(template);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var templates = await _templateStore.GetAll(prefix, cancellationToken);
        return new OkObjectResult(templates);
    }

    [HttpPut]
    public async Task<IActionResult> Enable([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Templates.Name);
            var templates = await _templateStore.GetAll(prefix, cancellationToken);
            var existingTemplate = templates.SingleOrDefault(t => t.Id == id);
            if (existingTemplate == null)
            {
                return new NotFoundResult();
            }

            existingTemplate.IsActive = true;
            var otherTemplates = templates.Where(t => t.Id != id);
            foreach (var o in otherTemplates)
            {
                o.IsActive = false;
            }

            await _templateStore.SaveChanges(cancellationToken);
            return new NoContentResult();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] FormBuilder.Models.Template template, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Templates.Name);
            var existingTemplate = await _templateStore.Get(id, cancellationToken);
            if (existingTemplate == null)
            {
                return new NotFoundResult();
            }

            var existingStyles = existingTemplate.Styles;
            foreach (var s in template.Styles)
            {
                var existingStyle = existingStyles.Single(st => st.Id == s.Id);
                if (existingStyle != null)
                {
                    existingStyle.Value = s.Value;
                }
            }

            existingTemplate.Styles = existingStyles.ToList();
            existingTemplate.Windows = template.Windows;
            existingTemplate.Elements = template.Elements;
            await _templateStore.SaveChanges(cancellationToken);
            return new NoContentResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}