// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Helpers;
using FormBuilder.Models;
using FormBuilder.Repositories;
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

    public WorkflowsController(ILogger<WorkflowsController> logger, IWorkflowStore workflowStore, IDateTimeHelper dateTimeHelper, ITokenRepository tokenRepository, IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _logger = logger;
        _workflowStore = workflowStore;
        _dateTimeHelper = dateTimeHelper;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            prefix = prefix ?? Constants.DefaultRealm;
            await CheckAccessToken(prefix, Constants.DefaultScopes.Workflows.Name);
            var workflow = await _workflowStore.Get(prefix, id, cancellationToken);
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
            await CheckAccessToken(prefix, Constants.DefaultScopes.Workflows.Name);
            var workflow = await _workflowStore.Get(prefix, id, cancellationToken);
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
}
