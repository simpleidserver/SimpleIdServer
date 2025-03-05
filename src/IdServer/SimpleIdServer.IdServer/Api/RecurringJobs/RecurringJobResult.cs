// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.IdServer.Api.RecurringJobs;

public class RecurringJobResult
{
    public string Id
    {
        get; set;
    }

    public DateTime? LastExecution
    {
        get; set;
    }

    public string LastJobState
    {
        get; set;
    }

    public string Error
    {
        get; set;
    }

    public int RetryAttempt
    {
        get; set;
    }

    public DateTime? NextExecution
    {
        get; set;
    }

    public string Cron
    {
        get; set;
    }
}
