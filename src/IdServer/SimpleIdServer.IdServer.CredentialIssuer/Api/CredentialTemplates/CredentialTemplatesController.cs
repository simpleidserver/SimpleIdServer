// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.CredentialIssuer.ExternalEvents;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.Vc.Builders;
using SimpleIdServer.Vc.DTOs;
using SimpleIdServer.Vc.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialTemplates;

[AllowAnonymous]
public class CredentialTemplatesController : BaseController
{
    private readonly ICredentialTemplateRepository _credentialTemplateRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IBusControl _busControl;
    private readonly ILogger<CredentialTemplatesController> _logger;

    public CredentialTemplatesController(
        ICredentialTemplateRepository credentialTemplateRepository, 
        IRealmRepository realmRepository, 
        IBusControl busControl,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder, 
        ILogger<CredentialTemplatesController> logger) : base(tokenRepository, jwtBuilder)
    {
        _credentialTemplateRepository = credentialTemplateRepository;
        _realmRepository = realmRepository;
        _busControl = busControl;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.CredentialTemplates.Name);
            var query = _credentialTemplateRepository
                .Query()
                .Include(c => c.Parameters)
                .Include(c => c.Realms)
                .Include(c => c.DisplayLst)
                .AsNoTracking()
                .Where(c => c.Realms.Any(r => r.Name == prefix));
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                query = query.OrderBy(request.OrderBy);
            else
                query = query.OrderByDescending(c => c.UpdateDateTime);

            var nb = query.Count();
            var result = await query.ToListAsync(CancellationToken.None);
            return new OkObjectResult(new SearchResult<CredentialTemplate>
            {
                Content = result,
                Count = nb
            });
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Remove([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove credential template"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.CredentialTemplates.Name);
                var result = await _credentialTemplateRepository
                    .Query()
                    .Include(c => c.Realms)
                    .SingleOrDefaultAsync(c => c.Realms.Any(r => r.Name == prefix) && c.Id == id);
                if (result == null) throw new OAuthException(System.Net.HttpStatusCode.NotFound, IdServer.ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_TEMPLATE, id));
                _credentialTemplateRepository.Delete(result);
                await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Credential template {id} is removed");
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddW3CredentialTemplate([FromRoute] string prefix, [FromBody] AddW3CCredentialTemplateRequest request)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add W3C credential template"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                if (request == null) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, IdServer.ErrorCodes.INVALID_REQUEST, IdServer.ErrorMessages.INVALID_INCOMING_REQUEST);
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, IdServer.ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, CredentialTemplateNames.Name));
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, IdServer.ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, CredentialTemplateNames.Name));
                if (string.IsNullOrWhiteSpace(request.Type)) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, IdServer.ErrorCodes.INVALID_REQUEST, string.Format(IdServer.ErrorMessages.MISSING_PARAMETER, CredentialTemplateNames.Type));
                await CheckAccessToken(prefix, Constants.StandardScopes.CredentialTemplates.Name);
                var existingCredentialTemplate = await _credentialTemplateRepository.Query()
                    .Include(c => c.Parameters)
                    .AnyAsync(t => t.Format == Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials && t.Parameters.Any(p => p.Name == "type" && p.Value == request.Type));
                if (existingCredentialTemplate) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, IdServer.ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.EXISTING_CREDENTIAL_TEMPLATE, request.Type));
                var w3CCredentialTemplate = W3CCredentialTemplateBuilder.New(request.Name, request.LogoUrl, request.Type).Build();
                var credentialTemplate = new CredentialTemplate(w3CCredentialTemplate);
                var realm = await _realmRepository.Query().FirstAsync(r => r.Name == prefix);
                credentialTemplate.Realms.Add(realm);
                _credentialTemplateRepository.Add(credentialTemplate);
                await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
                await _busControl.Publish(new CredentialTemplateAddedSuccessEvent
                {
                    Realm = prefix,
                    Id = credentialTemplate.Id,
                    Type = request.Type,
                    Name = request.Name,
                    LogoUrl = request.LogoUrl
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(credentialTemplate).ToString(),
                    ContentType = "application/json"
                };
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                await _busControl.Publish(new CredentialTemplateAddedFailureEvent
                {
                    Realm = prefix,
                    LogoUrl = request?.LogoUrl,
                    Name = request?.Name,
                    Type = request?.Type
                });
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.CredentialTemplates.Name);
            var result = await _credentialTemplateRepository
                .Query()
                .Include(c => c.Realms)
                .Include(c => c.DisplayLst)
                .Include(c => c.Parameters)
                .SingleOrDefaultAsync(c => c.Realms.Any(r => r.Name == prefix) && c.TechnicalId == id);
            if (result == null) throw new OAuthException(System.Net.HttpStatusCode.NotFound, IdServer.ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_TEMPLATE, id));
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveDisplay([FromRoute] string prefix, string id, string displayId)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove credential template display"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.CredentialTemplates.Name);
                var result = await _credentialTemplateRepository
                    .Query()
                    .Include(c => c.DisplayLst)
                    .Include(c => c.Realms)
                    .SingleOrDefaultAsync(c => c.Realms.Any(r => r.Name == prefix) && c.TechnicalId == id);
                if (result == null) throw new OAuthException(System.Net.HttpStatusCode.NotFound, IdServer.ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_TEMPLATE, id));
                var display = result.DisplayLst.SingleOrDefault(d => d.Id == displayId);
                if(display == null) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, IdServer.ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_DISPLAY_TEMPLATE, displayId));
                result.DisplayLst.Remove(display);
                await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Credential template display {displayId} is removed");
                await _busControl.Publish(new CredentialTemplateDisplayRemovedSuccessEvent
                {
                    Realm = prefix,
                    Id = id,
                    DisplayId = displayId
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new CredentialTemplateDisplayRemovedFailureEvent
                {
                    Realm = prefix,
                    Id = id,
                    DisplayId = displayId
                });
                return BuildError(ex);
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddDisplay([FromRoute] string prefix, string id, [FromBody] AddCredentialTemplateDisplayRequest request)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add credential template display"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.CredentialTemplates.Name);
                var result = await _credentialTemplateRepository
                    .Query()
                    .Include(c => c.DisplayLst)
                    .Include(c => c.Realms)
                    .SingleOrDefaultAsync(c => c.Realms.Any(r => r.Name == prefix) && c.TechnicalId == id);
                if (result == null) throw new OAuthException(System.Net.HttpStatusCode.NotFound, IdServer.ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_TEMPLATE, id));
                var display = new CredentialTemplateDisplay
                {
                    BackgroundColor = request.BackgroundColor,
                    Description = request.Description,
                    Id = Guid.NewGuid().ToString(),
                    Locale = request.Locale,
                    LogoUrl = request.LogoUrl,
                    LogoAltText = request.LogoAltText,
                    Name = request.Name,
                    TextColor = request.TextColor
                };
                result.DisplayLst.Add(display);
                await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Credential template display {display.Id} is added");
                await _busControl.Publish(new CredentialTemplateDisplayAddedSuccessEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = JsonSerializer.Serialize(display).ToString(),
                    ContentType = "application/json"
                };
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new CredentialTemplateDisplayAddedFailureEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return BuildError(ex);
            }
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveParameter([FromRoute] string prefix, string id, string parameterId)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove credential template parameter"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.CredentialTemplates.Name);
                var result = await _credentialTemplateRepository
                    .Query()
                    .Include(c => c.Parameters)
                    .Include(c => c.Realms)
                    .SingleOrDefaultAsync(c => c.Realms.Any(r => r.Name == prefix) && c.TechnicalId == id);
                if (result == null) throw new OAuthException(System.Net.HttpStatusCode.NotFound, IdServer.ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_TEMPLATE, id));
                var parameter = result.Parameters.SingleOrDefault(d => d.Id == parameterId);
                if (parameter == null) throw new OAuthException(System.Net.HttpStatusCode.BadRequest, IdServer.ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_DISPLAY_PARAMETER, parameterId));
                result.Parameters.Remove(parameter);
                await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Credential template parameter {parameter} is removed");
                await _busControl.Publish(new CredentialTemplateParameterRemovedSuccessEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new CredentialTemplateParameterRemovedFailureEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return BuildError(ex);
            }
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateParameters([FromRoute] string prefix, string id, [FromBody] UpdateCredentialTemplateParametersRequest request)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Update credential template parameters"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                await CheckAccessToken(prefix, Constants.StandardScopes.CredentialTemplates.Name);
                var credentialTemplate = await _credentialTemplateRepository
                    .Query()
                    .Include(c => c.Parameters)
                    .Include(c => c.Realms)
                    .SingleOrDefaultAsync(c => c.Realms.Any(r => r.Name == prefix) && c.TechnicalId == id);
                if (credentialTemplate == null) throw new OAuthException(System.Net.HttpStatusCode.NotFound, IdServer.ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_TEMPLATE, id));
                var result = new List<CredentialTemplateParameter>();
                if(request.Parameters != null &&  request.Parameters.Any())
                {
                    foreach(var parameter in request.Parameters)
                    {
                        var existingParameter = credentialTemplate.Parameters.SingleOrDefault(p => p.Name == parameter.Name);
                        if(existingParameter != null)
                        {
                            existingParameter.ParameterType = parameter.ParameterType;
                            existingParameter.Value = parameter.Value;
                            existingParameter.IsArray = parameter.IsArray;
                        }
                        else
                        {
                            parameter.Id = Guid.NewGuid().ToString();
                            credentialTemplate.Parameters.Add(parameter);
                        }

                        result.Add(parameter);
                    }
                }

                await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"Credential template parameters are updated");
                await _busControl.Publish(new CredentialTemplateParametersUpdatedSuccessEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new CredentialTemplateParametersUpdatedFailureEvent
                {
                    Realm = prefix,
                    Id = id
                });
                return BuildError(ex);
            }
        }
    }
}