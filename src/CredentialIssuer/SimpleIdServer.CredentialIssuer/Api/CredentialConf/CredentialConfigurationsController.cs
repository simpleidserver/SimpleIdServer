// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialConf;

[Route(Constants.EndPoints.CredentialConfigurations)]
[Authorize("credconfs")]
public class CredentialConfigurationsController : BaseController
{
    private readonly ICredentialConfigurationStore _credentialConfigurationStore;
    private readonly IEnumerable<ICredentialFormatter> _formatters;

    public CredentialConfigurationsController(ICredentialConfigurationStore credentialConfigurationStore, IEnumerable<ICredentialFormatter> formatters)
    {
        _credentialConfigurationStore = credentialConfigurationStore;
        _formatters = formatters;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var credentialConfigurations = await _credentialConfigurationStore.GetAll(cancellationToken);
        return new OkObjectResult(credentialConfigurations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        return new OkObjectResult(credentialConfiguration);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCredentialConfigurationDetailsRequest request, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        if (!TryValidate(request, out ErrorResult? error))
            return Build(error.Value);
        var serverId = $"{request.Type}_{request.Format}";
        var existingCredentialConfiguration = await _credentialConfigurationStore.GetByServerId(serverId, cancellationToken);
        if (existingCredentialConfiguration != null && existingCredentialConfiguration.Id != id)
            return Build(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.EXISTING_CREDENTIAL_CONFIGURATION, serverId)));

        credentialConfiguration.JsonLdContext = request.JsonLdContext;
        credentialConfiguration.BaseUrl = request.BaseUrl;
        credentialConfiguration.Type = request.Type;
        credentialConfiguration.Scope = request.Scope;
        credentialConfiguration.Format = request.Format;
        credentialConfiguration.ServerId = serverId;
        credentialConfiguration.UpdateDateTime = DateTime.UtcNow;
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new ContentResult
        {
            ContentType = "application/json",
            Content = JsonSerializer.Serialize(credentialConfiguration),
            StatusCode = (int)HttpStatusCode.OK
        };
    }

    [HttpPost("{id}/displays")]
    public async Task<IActionResult> AddDisplay(string id, [FromBody] CredentialConfigurationDisplayRequest request, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        if (!TryValidate(request, out ErrorResult? error))
            return Build(error.Value);
        var otherDisplaySameLanguage = credentialConfiguration.Displays.SingleOrDefault(d => d.Locale == request.Locale);
        if (otherDisplaySameLanguage != null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.EXISTING_DISPLAY_SAME_LANGUAGE));

        var record = new CredentialConfigurationTranslation
        {
            Id = Guid.NewGuid().ToString(),
            Locale = request.Locale,
            BackgroundColor = request.BackgroundColor,
            Description = request.Description,
            LogoAltText = request.LogoAltText,
            LogoUrl = request.LogoUrl,
            Name = request.Name,
            TextColor = request.TextColor
        };
        credentialConfiguration.UpdateDateTime = DateTime.UtcNow;
        credentialConfiguration.Displays.Add(record);
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new ContentResult
        {
            ContentType = "application/json",
            Content = JsonSerializer.Serialize(record),
            StatusCode = (int)HttpStatusCode.Created
        };
    }

    [HttpPut("{id}/displays/{displayId}")]
    public async Task<IActionResult> UpdateDisplay(string id, string displayId, [FromBody] CredentialConfigurationDisplayRequest request, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        if (!TryValidate(request, out ErrorResult? error))
            return Build(error.Value);
        var existingDisplay = credentialConfiguration.Displays.SingleOrDefault(d => d.Id == displayId);
        if (existingDisplay == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION_DISPLAY, id)));
        var otherDisplaySameLanguage = credentialConfiguration.Displays.SingleOrDefault(d => d.Locale == request.Locale && d.Id != displayId);
        if(otherDisplaySameLanguage != null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.EXISTING_DISPLAY_SAME_LANGUAGE));

        existingDisplay.Locale = request.Locale;
        existingDisplay.BackgroundColor = request.BackgroundColor;
        existingDisplay.Description = request.Description;
        existingDisplay.LogoAltText = request.LogoAltText;
        existingDisplay.LogoUrl = request.LogoUrl;
        existingDisplay.Name = request.Name;
        existingDisplay.TextColor = request.TextColor;
        credentialConfiguration.UpdateDateTime = DateTime.UtcNow;
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new ContentResult
        {
            ContentType = "application/json",
            StatusCode = (int)HttpStatusCode.NoContent
        };
    }

    [HttpDelete("{id}/displays/{displayId}")]
    public async Task<IActionResult> DeleteDisplay(string id, string displayId, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        var existingDisplay = credentialConfiguration.Displays.SingleOrDefault(d => d.Id == displayId);
        if (existingDisplay == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION_DISPLAY, id)));
        credentialConfiguration.Displays.Remove(existingDisplay);
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.NoContent
        };
    }

    [HttpPost("{id}/claims")]
    public async Task<IActionResult> AddClaim(string id, [FromBody] CredentialConfigurationClaimRequest request, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        if (!TryValidate(request, out ErrorResult? error))
            return Build(error.Value);
        var existingClaim = credentialConfiguration.Claims.SingleOrDefault(c => c.Name == request.Name);
        if (existingClaim == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.EXISTING_CREDENTIAL_CLAIM, request.Name)));
        var claim = new CredentialConfigurationClaim
        {
            Id = Guid.NewGuid().ToString(),
            Mandatory = request.Mandatory,
            Name = request.Name,
            SourceUserClaimName = request.SourceUserClaimName,
            ValueType = request.ValueType
        };
        credentialConfiguration.Claims.Add(claim);
        credentialConfiguration.UpdateDateTime = DateTime.UtcNow;
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new ContentResult
        {
            ContentType = "application/json",
            Content = JsonSerializer.Serialize(claim),
            StatusCode = (int)HttpStatusCode.Created
        };
    }

    [HttpDelete("{id}/claims/{claimId}")]
    public async Task<IActionResult> DeleteClaim(string id, string claimId, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        var claim = credentialConfiguration.Claims.SingleOrDefault(c => c.Id == claimId);
        if (claim == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CLAIM, claimId)));
        credentialConfiguration.Claims.Remove(claim);
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new NoContentResult();
    }

    [HttpPost("{id}/claims/{claimId}/translations")]
    public async Task<IActionResult> AddClaimTranslation(string id, string claimId, [FromBody] CredentialConfigurationClaimDisplayRequest request, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        var claim = credentialConfiguration.Claims.SingleOrDefault(c => c.Id == claimId);
        if (claim == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CLAIM, claimId)));
        if (!TryValidate(request, out ErrorResult? error))
            return Build(error.Value);
        var otherDisplaySameLanguage = claim.Translations.SingleOrDefault(d => d.Locale == request.Locale);
        if (otherDisplaySameLanguage != null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.EXISTING_DISPLAY_SAME_LANGUAGE));
        var record = new CredentialConfigurationTranslation
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Locale = request.Locale
        };
        claim.Translations.Add(record);
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new ContentResult
        {
            ContentType = "application/json",
            Content = JsonSerializer.Serialize(record),
            StatusCode = (int)HttpStatusCode.Created
        };
    }

    [HttpDelete("{id}/claims/{claimId}/translations/{translationId}")]
    public async Task<IActionResult> AddClaimTranslation(string id, string claimId, string translationId, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        var claim = credentialConfiguration.Claims.SingleOrDefault(c => c.Id == claimId);
        if (claim == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CLAIM, claimId)));
        var translation = claim.Translations.SingleOrDefault(t => t.Id == translationId);
        if (translation == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CLAIM_TRANSLATION, translationId)));
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new NoContentResult();
    }

    [HttpPut("{id}/claims/{claimId}/translations/{translationId}")]
    public async Task<IActionResult> UpdateClaimTranslation(string id, string claimId, string translationId, [FromBody] CredentialConfigurationClaimDisplayRequest request, CancellationToken cancellationToken)
    {
        var credentialConfiguration = await _credentialConfigurationStore.Get(id, cancellationToken);
        if (credentialConfiguration == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CONFIGURATION, id)));
        var claim = credentialConfiguration.Claims.SingleOrDefault(c => c.Id == claimId);
        if (claim == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CLAIM, claimId)));
        var translation = claim.Translations.SingleOrDefault(t => t.Id == translationId);
        if (translation == null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_CLAIM_TRANSLATION, translationId)));
        if (!TryValidate(request, out ErrorResult? error))
            return Build(error.Value);
        var otherDisplaySameLanguage = claim.Translations.SingleOrDefault(d => d.Locale == request.Locale);
        if (otherDisplaySameLanguage != null)
            return Build(new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.EXISTING_DISPLAY_SAME_LANGUAGE));
        translation.Name = request.Name;
        translation.Locale = request.Locale;
        await _credentialConfigurationStore.SaveChanges(cancellationToken);
        return new NoContentResult();
    }

    private bool TryValidate(UpdateCredentialConfigurationDetailsRequest request, out ErrorResult? error)
    {
        error = null;
        if (request == null)
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);

        if (string.IsNullOrWhiteSpace(request.JsonLdContext))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Join(ErrorMessages.MISSING_PARAMETER, "json_ld_context"));

        if (string.IsNullOrWhiteSpace(request.BaseUrl))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Join(ErrorMessages.MISSING_PARAMETER, "base_url"));

        if (string.IsNullOrWhiteSpace(request.Type))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Join(ErrorMessages.MISSING_PARAMETER, "type"));

        if (string.IsNullOrWhiteSpace(request.Format))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Join(ErrorMessages.MISSING_PARAMETER, "format"));

        var formatter = _formatters.SingleOrDefault(f => f.Format == request.Format);
        if (formatter == null)
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_FORMAT, request.Format));

        return error == null;
    }

    private bool TryValidate(CredentialConfigurationDisplayRequest request, out ErrorResult? error)
    {
        error = null;
        if (request == null)
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
        if (string.IsNullOrWhiteSpace(request.Locale))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Join(ErrorMessages.MISSING_PARAMETER, "locale"));
        return error == null;
    }

    private bool TryValidate(CredentialConfigurationClaimRequest request, out ErrorResult? error)
    {
        error = null;
        if (request == null)
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
        if (string.IsNullOrWhiteSpace(request.SourceUserClaimName))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, "source_claim_name"));
        if (string.IsNullOrWhiteSpace(request.Name))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, "name"));
        return error == null;
    }

    private bool TryValidate(CredentialConfigurationClaimDisplayRequest request, out ErrorResult? error)
    {
        error = null;
        if (request == null)
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
        if (string.IsNullOrWhiteSpace(request.Name))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, "name"));
        if (string.IsNullOrWhiteSpace(request.Locale))
            error = new ErrorResult(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, "locale"));
        return error == null;
    }
}