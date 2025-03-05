// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.RecurringJobs;

public class RecurringJobsController : BaseController
{
    private readonly JobStorage _jobStorage;
    private readonly ILogger<RecurringJobsController> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RecurringJobsController(
        JobStorage jobStorage, 
        ILogger<RecurringJobsController> logger, 
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder,
        IServiceProvider serviceProvider) : base(tokenRepository, jwtBuilder)
    {
        _jobStorage = jobStorage;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        using (var connection = _jobStorage.GetConnection())
        {
            var recurringJobs = connection.GetRecurringJobs();
            var result = recurringJobs.Select(j => new RecurringJobResult
            {
                Id = j.Id,
                LastExecution = j.LastExecution,
                LastJobState = j.LastJobState,
                Error = j.Error,
                RetryAttempt = j.RetryAttempt,
                NextExecution = j.NextExecution,
                Cron = j.Cron
            }).ToList();
            return Ok(result);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] UpdateJobParameter parameter)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        if (parameter == null)
        {
            return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidRequestParameter);
        }

        if(string.IsNullOrWhiteSpace(parameter.Cron))
        {
            return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, nameof(UpdateJobParameter.Cron)));
        }

        try
        {
            await CheckAccessToken(prefix, Constants.StandardScopes.RecurringJobs.Name);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }

        using (var connection = _jobStorage.GetConnection())
        {
            var recurringJob = connection.GetRecurringJobs().FirstOrDefault(r => r.Id == id);
            if (recurringJob == null)
            {
                return BuildError(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownRecurringJob, id));
            }

            var lambdaExpression = BuildLambdaExpression(recurringJob.Job);
            RecurringJob.AddOrUpdate(
                    id,
                    lambdaExpression,
                    parameter.Cron,
                    TimeZoneInfo.Local,
                    recurringJob.Queue ?? "default");
            return NoContent();
        }
    }

    private Expression<Action> BuildLambdaExpression(Hangfire.Common.Job job)
    {
        var methodInfo = job.Method;
        var arguments = job.Arguments;

        var parameterExpressions = methodInfo.GetParameters()
            .Select((p, i) => Expression.Constant(arguments[i], p.ParameterType))
            .ToArray();

        var serviceProviderExpr = Expression.Constant(_serviceProvider);
        var getServiceMethod = typeof(IServiceProvider).GetMethod("GetService");
        var typeExpr = Expression.Constant(methodInfo.DeclaringType);
        var getServiceCall = Expression.Call(serviceProviderExpr, getServiceMethod, typeExpr);
        var instanceExpression = Expression.Convert(getServiceCall, methodInfo.DeclaringType);

        var methodCall = Expression.Call(instanceExpression, methodInfo, parameterExpressions);
        return Expression.Lambda<Action>(methodCall);
    }
}
