// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultRegistrationWorkflowRepository : IRegistrationWorkflowRepository
{
    private readonly List<RegistrationWorkflow> _registrationWorkflows;

    public DefaultRegistrationWorkflowRepository(List<RegistrationWorkflow> registrationWorkflows)
    {
        _registrationWorkflows = registrationWorkflows;
    }

    public Task<RegistrationWorkflow> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var result = _registrationWorkflows.SingleOrDefault(r => r.RealmName == realm && r.Id == id);
        return Task.FromResult(result);
    }

    public Task<RegistrationWorkflow> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        var result = _registrationWorkflows.SingleOrDefault(r => r.RealmName == realm && r.Name == name);
        return Task.FromResult(result);
    }

    public Task<RegistrationWorkflow> GetDefault(string realm, CancellationToken cancellationToken)
    {
        var result = _registrationWorkflows.SingleOrDefault(r => r.RealmName == realm && r.IsDefault);
        return Task.FromResult(result);
    }

    public Task<List<RegistrationWorkflow>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = _registrationWorkflows
                        .Where(r => r.RealmName == realm)
                        .OrderByDescending(r => r.UpdateDateTime)
                        .ToList();
        return Task.FromResult(result);
    }

    public Task<List<RegistrationWorkflow>> GetByIds(string realm, List<string> ids, CancellationToken cancellationToken)
    {
        var result = _registrationWorkflows
                        .Where(r => r.RealmName == realm && ids.Contains(r.Id))
                        .OrderByDescending(r => r.UpdateDateTime)
                        .ToList();
        return Task.FromResult(result);
    }

    public void Delete(RegistrationWorkflow record) => _registrationWorkflows.Remove(record);

    public void Add(RegistrationWorkflow record) => _registrationWorkflows.Add(record);

    public void Update(RegistrationWorkflow record)
    {
        var index = _registrationWorkflows.FindIndex(r => r.RealmName == record.RealmName && r.Id == record.Id);
        if (index != -1)
            _registrationWorkflows[index] = record;
    }
}
