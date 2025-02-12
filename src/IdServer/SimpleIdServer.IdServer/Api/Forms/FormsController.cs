// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FormBuilder.Helpers;
using FormBuilder.Models;
using FormBuilder.Services;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Forms;

public class FormsController : BaseController
{
    private readonly IFormStore _formStore;
    private readonly ILogger<FormsController> _logger;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IVersionedFormService _versionedFormService;

    public FormsController(IFormStore formStore, ILogger<FormsController> logger, IDateTimeHelper dateTimeHelper, IVersionedFormService versionedFormService, ITokenRepository tokenRepository, IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _formStore = formStore;
        _logger = logger;
        _dateTimeHelper = dateTimeHelper;
        _versionedFormService = versionedFormService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Constants.StandardScopes.Forms.Name);
            var form = await _formStore.GetLatestVersionByCorrelationId(prefix, id, cancellationToken);
            if (form == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownForm, id));
            return new OkObjectResult(form);
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] FormRecord form, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Constants.StandardScopes.Forms.Name);
            var existingForm = await _formStore.GetLatestVersionByCorrelationId(prefix, id, cancellationToken);
            if (existingForm == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownForm, id));
            existingForm.Update(form.Elements.ToList(), _dateTimeHelper.GetCurrent());
            await _formStore.SaveChanges(cancellationToken);
            return new NoContentResult();
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Publish([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Constants.StandardScopes.Forms.Name);
            var existingForm = await _formStore.GetLatestVersionByCorrelationId(prefix, id, cancellationToken);
            if (existingForm == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownForm, id));
            var newForm = await _versionedFormService.Publish(existingForm, cancellationToken);
            return new OkObjectResult(newForm);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCss([FromRoute] string prefix, string id, [FromBody] UpdateCssStyleCommand cmd, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Constants.StandardScopes.Forms.Name);
            var existingForm = await _formStore.GetLatestVersionByCorrelationId(prefix, id, cancellationToken);
            if (existingForm == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownForm, id));
            existingForm.ActiveStyle.Content = cmd.Css;
            await _formStore.SaveChanges(cancellationToken);
            return new NoContentResult();
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}
