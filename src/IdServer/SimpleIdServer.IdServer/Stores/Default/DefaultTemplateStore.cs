// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using FormBuilder.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultTemplateStore : ITemplateStore
{
    private readonly List<Template> _templates = new List<Template>();

    public DefaultTemplateStore()
    {

    }

    public void Add(Template record)
    {
        _templates.Add(record);
    }

    public Task<Template> Get(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_templates.SingleOrDefault(t => t.Id == id));
    }

    public Task<Template> GetActive(string realm, CancellationToken cancellationToken)
    {
        return Task.FromResult(_templates.SingleOrDefault(t => t.Realm == realm && t.IsActive));
    }

    public Task<List<Template>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return Task.FromResult(_templates.Where(t => t.Realm == realm).ToList());
    }

    public Task<List<Template>> GetByName(string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(_templates.Where(t => t.Name == name).ToList());
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}
