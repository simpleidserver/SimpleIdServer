// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using FormBuilder.Stores;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.EF.Stores;

public class TemplateStore : ITemplateStore
{
    private readonly FormBuilderDbContext _dbcontext;

    public TemplateStore(FormBuilderDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public void Add(Template record)
    {
        _dbcontext.Templates.Add(record);
    }

    public Task<Template> Get(string id, CancellationToken cancellationToken)
    {
        return _dbcontext.Templates.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<Template> GetActive(string realm, CancellationToken cancellationToken)
    {
        return _dbcontext.Templates.SingleOrDefaultAsync(t => t.Realm == realm && t.IsActive, cancellationToken);
    }

    public Task<List<Template>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return _dbcontext.Templates.Where(t => t.Realm == realm).ToListAsync(cancellationToken);
    }

    public Task<List<Template>> GetByName(string name, CancellationToken cancellationToken)
    {
        return _dbcontext.Templates.Where(t => t.Name == name).ToListAsync(cancellationToken);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return _dbcontext.SaveChangesAsync(cancellationToken);
    }
}
