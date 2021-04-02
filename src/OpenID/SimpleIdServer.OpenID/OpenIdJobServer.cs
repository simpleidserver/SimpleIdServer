// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Infrastructures.Jobs;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID
{
    public class OpenIdJobServer : IOpenIdJobServer
    {
        private readonly IEnumerable<IJob> _jobs;

        public OpenIdJobServer(IEnumerable<IJob> jobs)
        {
            _jobs = jobs;
        }

        public void Start()
        {
            foreach (var job in _jobs)
            {
                job.Start();
            }
        }

        public void Stop()
        {
            foreach (var job in _jobs)
            {
                try
                {
                    job.Cancel().Wait();
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
