// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.RecurringJobs;

public class RecurringJobsController : BaseController
{
    private readonly JobStorage _jobStorage;
    private readonly ILogger<RecurringJobsController> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMonitoringApi _monitoringApi;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IRecurringJobStatusRepository _recurringJobStatusRepository;
    private readonly ITransactionBuilder _transactionBuilder;

    public RecurringJobsController(
        JobStorage jobStorage, 
        ILogger<RecurringJobsController> logger, 
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder,
        IServiceProvider serviceProvider,
        IRecurringJobManager recurringJobManager,
        IRecurringJobStatusRepository recurringJobStatusRepository,
        ITransactionBuilder transactionBuilder) : base(tokenRepository, jwtBuilder)
    {
        _jobStorage = jobStorage;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _monitoringApi = jobStorage.GetMonitoringApi();
        _recurringJobManager = recurringJobManager;
        _recurringJobStatusRepository = recurringJobStatusRepository;
        _transactionBuilder = transactionBuilder;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        using (var connection = _jobStorage.GetConnection())
        {
            var recurringJobs = connection.GetRecurringJobs();
            var allStatus = await _recurringJobStatusRepository.Get(recurringJobs.Select(r => r.Id).ToList(), cancellationToken);
            var result = recurringJobs.Select(j => new RecurringJobResult
            {
                Id = j.Id,
                LastExecution = j.LastExecution,
                LastJobState = j.LastJobState,
                Error = j.Error,
                RetryAttempt = j.RetryAttempt,
                NextExecution = j.NextExecution,
                Cron = j.Cron,
                IsDisabled = allStatus.SingleOrDefault(s => s.JobId == j.Id)?.IsDisabled ?? false
                
            }).ToList();
            return Ok(result);
        }
    }

    [HttpGet]
    public IActionResult GetLastFailedJobs([FromRoute] string prefix)
    {
        var result = _monitoringApi.FailedJobs(0, 100).Select(j => new FailedJobResult
        {
            ExceptionDetails = j.Value.ExceptionDetails,
            ExceptionMessage = j.Value.ExceptionMessage,
            ExceptionType = j.Value.ExceptionType,
            FailedAt = j.Value.FailedAt,
            InFailedState = j.Value.InFailedState,
            Method = j.Value.Job.Type.Name,
            Reason = j.Value.Reason
        });
        return new OkObjectResult(result);
    }

    [HttpPost]
    public Task<IActionResult> Enable([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        return Toggle(prefix, id, false, cancellationToken);
    }

    [HttpDelete]
    public Task<IActionResult> Disable([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        return Toggle(prefix, id, true, cancellationToken);
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
            await CheckAccessToken(prefix, Constants.DefaultScopes.RecurringJobs.Name);
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

    [HttpGet]
    public async Task<IActionResult> Launch([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.DefaultScopes.RecurringJobs.Name);
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

            _recurringJobManager.Trigger(id);
            return NoContent();
        }
    }

    private async Task<IActionResult> Toggle(string prefix, string id, bool isDisabled, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Constants.DefaultScopes.RecurringJobs.Name);
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

            using (var transaction = _transactionBuilder.Build())
            {
                var recurringJobStatus = await _recurringJobStatusRepository.Get(id, cancellationToken);
                if (recurringJobStatus == null)
                {
                    recurringJobStatus = new RecurringJobStatus
                    {
                        JobId = id,
                        IsDisabled = isDisabled
                    };
                    _recurringJobStatusRepository.Add(recurringJobStatus);
                }
                recurringJobStatus.IsDisabled = isDisabled;
                await transaction.Commit(cancellationToken);
            }

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
        var typeExpr = Expression.Constant(methodInfo.ReflectedType);
        var getServiceCall = Expression.Call(serviceProviderExpr, getServiceMethod, typeExpr);
        var instanceExpression = Expression.Convert(getServiceCall, methodInfo.DeclaringType);

        var methodCall = Expression.Call(instanceExpression, methodInfo, parameterExpressions);
        return Expression.Lambda<Action>(methodCall);
    }
}
