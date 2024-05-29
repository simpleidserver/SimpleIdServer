// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IRegistrationWorkflowRepository
{
    Task<RegistrationWorkflow> Get(string realm, string id, CancellationToken cancellationToken);
    Task<RegistrationWorkflow> GetByName(string realm, string name, CancellationToken cancellationToken);
    Task<RegistrationWorkflow> GetDefault(string realm, CancellationToken cancellationToken);
    Task<List<RegistrationWorkflow>> GetAll(string realm,  CancellationToken cancellationToken);
    void Delete(RegistrationWorkflow record);
    void Add(RegistrationWorkflow record);
    void Update(RegistrationWorkflow record);
}