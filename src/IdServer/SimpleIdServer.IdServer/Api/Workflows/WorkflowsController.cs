// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using FormBuilder.Models;
using FormBuilder.Repositories;
using FormBuilder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Workflows;

public class WorkflowsController : BaseController
{
    private readonly ILogger<WorkflowsController> _logger;
    private readonly IWorkflowStore _workflowStore;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IVersionedWorkflowService _versionedWorkflowService;

    public WorkflowsController(ILogger<WorkflowsController> logger, IWorkflowStore workflowStore, IDateTimeHelper dateTimeHelper, IVersionedWorkflowService versionedWorkflowService, ITokenRepository tokenRepository, IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _logger = logger;
        _workflowStore = workflowStore;
        _dateTimeHelper = dateTimeHelper;
        _versionedWorkflowService = versionedWorkflowService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Constants.StandardScopes.Workflows.Name);
            var workflow = await _workflowStore.GetLatestPublishedRecord(prefix, id, cancellationToken);
            if (workflow == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_WORKFLOW, string.Format(Global.UnknownWorkflow, id));
            return new OkObjectResult(workflow);
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] WorkflowRecord record, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Constants.StandardScopes.Workflows.Name);
            var workflow = await _workflowStore.GetLatestPublishedRecord(prefix, id, cancellationToken);
            if (workflow == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_WORKFLOW, string.Format(Global.UnknownWorkflow, id));
            workflow.Update(record.Steps, record.Links, _dateTimeHelper.GetCurrent());
            await _workflowStore.SaveChanges(cancellationToken);
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
            await CheckAccessToken(prefix, Constants.StandardScopes.Workflows.Name);
            var workflow = await _workflowStore.GetLatestPublishedRecord(prefix, id, cancellationToken);
            if (workflow == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.UNKNOWN_WORKFLOW, string.Format(Global.UnknownWorkflow, id));
            var publishedVersion = await _versionedWorkflowService.Publish(workflow, cancellationToken);
            return new OkObjectResult(publishedVersion);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}
