// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IRegistrationWorkflowRepository
{
    Task<RegistrationWorkflow> Get(string realm, string id, CancellationToken cancellationToken);
    IQueryable<RegistrationWorkflow> Query();
    void Delete(RegistrationWorkflow record);
    void Add(RegistrationWorkflow record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
