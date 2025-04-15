// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;

namespace FormBuilder.Stores;

public interface ITemplateStore
{
    Task<Template> Get(string id, CancellationToken cancellationToken);
    Task<Template> GetActive(string realm, CancellationToken cancellationToken);
    Task<List<Template>> GetByName(string name, CancellationToken cancellationToken);
    Task<List<Template>> GetAll(string realm, CancellationToken cancellationToken);
    void Add(Template record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
