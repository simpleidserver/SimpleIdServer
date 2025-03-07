// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.IdServer.Api.RecurringJobs;

public class FailedJobResult
{
    public string Method
    {
        get; set;
    }

    public string Reason { get; set; }

    public string ExceptionType { get; set; }

    public string ExceptionMessage { get; set; }

    public string ExceptionDetails { get; set; }

    public bool InFailedState { get; set; }

    public DateTime? FailedAt { get; set; }
}
