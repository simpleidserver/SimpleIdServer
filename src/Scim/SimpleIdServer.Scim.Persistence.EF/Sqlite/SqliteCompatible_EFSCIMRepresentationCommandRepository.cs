// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domains;

namespace SimpleIdServer.Scim.Persistence.EF.Sqlite;

public class SqliteCompatible_EFSCIMRepresentationCommandRepository : EFSCIMRepresentationCommandRepository, ISCIMRepresentationCommandRepository
{
    private readonly SCIMDbContext _scimDbContext;
    private readonly SCIMEFOptions _options;

    public SqliteCompatible_EFSCIMRepresentationCommandRepository(SCIMDbContext scimDbContext, IOptions<SCIMEFOptions> options) : base(scimDbContext, options)
    {
        _scimDbContext = scimDbContext;
        _options = options.Value;
    }

    public new async Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId, bool isReference = false)
    {
        scimRepresentationAttributes = scimRepresentationAttributes.Where(r => !string.IsNullOrWhiteSpace(r.RepresentationId)).ToList();
        var entitiesToDelete = _scimDbContext.SCIMRepresentationAttributeLst
            .Where(r1 => scimRepresentationAttributes.Select(x => x.Id).Any(id => r1.Id == id))
            .ToList();

        _scimDbContext.RemoveRange(entitiesToDelete);
    }

    public new async Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, bool isReference = false)
    {
        var updatedAttributes = scimRepresentationAttributes.ToDictionary(keySelector: x => x.Id);
        var attributeIds = updatedAttributes.Keys.ToHashSet();

        // Retrieve the entities to update
        var entitiesToUpdate = _scimDbContext.SCIMRepresentationAttributeLst
            .Where(attr => attributeIds.Contains(attr.Id))
            .ToList();

        // Update the properties of the retrieved entities
        foreach (var entity in entitiesToUpdate)
        {
            var valueFound = updatedAttributes.TryGetValue(entity.Id, out var updatedAttribute);
            if (!valueFound) continue;

            entity.ValueString = updatedAttribute.ValueString;
            entity.ValueDecimal = updatedAttribute.ValueDecimal;
            entity.ValueBoolean = updatedAttribute.ValueBoolean;
            entity.ValueBinary = updatedAttribute.ValueBinary;
            entity.ValueReference = updatedAttribute.ValueReference;
            entity.ValueInteger = updatedAttribute.ValueInteger;
            entity.ValueDateTime = updatedAttribute.ValueDateTime;
            entity.ComputedValueIndex = updatedAttribute.ComputedValueIndex;
        }

        _scimDbContext.UpdateRange(entitiesToUpdate);
    }
}
