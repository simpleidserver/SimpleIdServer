// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Jobs;
using System.Threading;

namespace SimpleIdServer.IdServer.Api.Provisioning
{
    public class IdentityProvisioningController : Controller
    {
        public IActionResult Enqueue(string id)
        {
            BackgroundJob.Enqueue<IIdentityProvisioningJob>((j) => j.Execute(id, CancellationToken.None));
            return new NoContentResult();
        }
    }
}
