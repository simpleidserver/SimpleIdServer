// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF;

public class EFSCIMRepresentationCommandRepository : ISCIMRepresentationCommandRepository
{
    private readonly SCIMDbContext _scimDbContext;
    private readonly ScimEfOptions _options;

    public EFSCIMRepresentationCommandRepository(SCIMDbContext scimDbContext, IOptions<ScimEfOptions> options)
    {
        _scimDbContext = scimDbContext;
        _options = options.Value;
    }

    public async Task<SCIMRepresentation> Get(string realm, string id, CancellationToken token = default)
    {
        var query = _scimDbContext.SCIMRepresentationLst
            .Include(r => r.Schemas).ThenInclude(s => s.Attributes);
        if(!string.IsNullOrWhiteSpace(realm))
            return await query.FirstOrDefaultAsync(r => r.RealmName == realm && r.Id == id, token);
        return await query.FirstOrDefaultAsync(r => r.Id == id, token);
    }

    public Task<bool> Add(SCIMRepresentation data, CancellationToken token)
    {
        _scimDbContext.SCIMRepresentationLst.Add(data);
        foreach (var attr in data.FlatAttributes)
            _scimDbContext.SCIMRepresentationAttributeLst.Add(attr);
        return Task.FromResult(true);
    }

    public Task<bool> Update(SCIMRepresentation data, CancellationToken token)
    {
        _scimDbContext.SCIMRepresentationLst.Update(data);
        return Task.FromResult(true);
    }

    public Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
    {
        _scimDbContext.SCIMRepresentationLst.Remove(data);
        return Task.FromResult(true);
    }

    public async Task<List<SCIMRepresentation>> FindRepresentations(List<string> representationIds, string resourceType = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var query = _scimDbContext.SCIMRepresentationLst
            .Where(r => representationIds.Contains(r.Id))
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(resourceType))
            query = query.Where(r => r.ResourceType == resourceType);
        var result = await query.ToListAsync(cancellationToken);
        return result;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindGraphAttributes(string valueStr, string schemaAttributeId, string sourceRepresentationId = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var parentIds = await _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
            .Where(a => a.SchemaAttributeId == schemaAttributeId && a.ValueString == valueStr || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
            .OrderBy(r => r.ParentAttributeId)
            .Select(r => r.ParentAttributeId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var result = await _scimDbContext.SCIMRepresentationAttributeLst.Include(s => s.SchemaAttribute).AsNoTracking()
                .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                .ToListAsync(cancellationToken);
        return result;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindGraphAttributes(IEnumerable<string> representationIds, List<string> values, string schemaAttributeId, string sourceRepresentationId = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var parentIds = await _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
            .Where(a => a.SchemaAttributeId == schemaAttributeId && (representationIds.Contains(a.RepresentationId) && values.Contains(a.ValueString)) || (sourceRepresentationId != null && a.ValueString == sourceRepresentationId))
            .Select(r => r.ParentAttributeId)
            .AsNoTracking()
            .Distinct()
            .ToListAsync(cancellationToken);
        var result = await _scimDbContext.SCIMRepresentationAttributeLst.Include(s => s.SchemaAttribute).AsNoTracking()
                .Where(a => parentIds.Contains(a.Id) || parentIds.Contains(a.ParentAttributeId))
                .ToListAsync();
        return result;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindGraphAttributesBySchemaAttributeId(string representationId, string schemaAttributeId, CancellationToken cancellationToken)
    {
        var ids = await _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
            .Where(a => a.SchemaAttributeId == schemaAttributeId && a.RepresentationId == representationId)
            .OrderBy(r => r.Id)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);
        var result = await _scimDbContext.SCIMRepresentationAttributeLst.Include(s => s.SchemaAttribute)
            .AsNoTracking()
            .Where(a => ids.Contains(a.Id) || ids.Contains(a.ParentAttributeId))
            .ToListAsync(cancellationToken);
        return result;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindGraphAttributesBySchemaAttributeId(List<string> representationIds, string schemaAttributeId, CancellationToken cancellationToken)
    {
        var ids = await _scimDbContext.SCIMRepresentationAttributeLst.AsNoTracking()
            .Where(a => a.SchemaAttributeId == schemaAttributeId && representationIds.Contains(a.RepresentationId))
            .OrderBy(r => r.Id)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);
        var result = await _scimDbContext.SCIMRepresentationAttributeLst.Include(s => s.SchemaAttribute).AsNoTracking()
            .Where(a => ids.Contains(a.Id) || ids.Contains(a.ParentAttributeId))
            .ToListAsync(cancellationToken);
        return result;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, SCIMAttributeExpression pathExpression, CancellationToken cancellationToken)
    {
        var representationAttributes = _scimDbContext.SCIMRepresentationAttributeLst
            .Include(s => s.SchemaAttribute)
            .Include(s => s.Children).ThenInclude(s => s.SchemaAttribute)
            .Where(r => r.RepresentationId == representationId)
            .AsNoTracking();
        if (pathExpression.SchemaAttribute == null || string.IsNullOrWhiteSpace(pathExpression.SchemaAttribute.Id))
            return new List<SCIMRepresentationAttribute>();
        var filteredAttributes = await pathExpression.EvaluateAttributes(representationAttributes, true, "Children").ToListAsync(cancellationToken);
        foreach (var a in filteredAttributes) a.CachedChildren = a.Children;
        return filteredAttributes.SelectMany(a => a.ToFlat()).ToList();
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributes(string representationId, CancellationToken cancellationToken)
    {
        var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.Where(r => r.RepresentationId == representationId).ToListAsync(cancellationToken);
        return representationAttributes;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributesByAproximativeFullPath(string representationId, string fullPath, CancellationToken cancellationToken)
    {
        var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.Include(a => a.SchemaAttribute).AsNoTracking()
            .Where(a => a.RepresentationId == representationId && a.FullPath.StartsWith(fullPath))
            .ToListAsync(cancellationToken);
        return representationAttributes;
    }
    
    public async Task<List<SCIMRepresentationAttribute>> FindAttributesByExactFullPathAndValues(string fullPath, IEnumerable<string> values, CancellationToken cancellationToken)
    {
        var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.Include(a => a.SchemaAttribute).AsNoTracking()
            .Where(a => values.Contains(a.ValueString) && a.FullPath == fullPath)
            .ToListAsync(cancellationToken);
        return representationAttributes;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributesByExactFullPathAndRepresentationIds(string fullPath, IEnumerable<string> values, CancellationToken cancellationToken)
    {
        var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.Include(a => a.SchemaAttribute).AsNoTracking()
            .Where(a => values.Contains(a.RepresentationId) && a.FullPath == fullPath)
            .ToListAsync(cancellationToken);
        return representationAttributes;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributesBySchemaAttributeAndValues(string schemaAttributeId, IEnumerable<string> values, CancellationToken cancellationToken)
    {
        var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.Include(a => a.SchemaAttribute).AsNoTracking()
            .Where(a => values.Contains(a.ValueString) && a.SchemaAttributeId == schemaAttributeId)
            .ToListAsync(cancellationToken);
        return representationAttributes;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributesByComputedValueIndexAndRepresentationId(List<string> computedValueIndexLst, string representationId, CancellationToken cancellationToken)
    {
        var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.Include(a => a.SchemaAttribute).AsNoTracking()
            .Where(a => computedValueIndexLst.Contains(a.ComputedValueIndex) && a.RepresentationId == representationId)
            .ToListAsync(cancellationToken);
        return representationAttributes;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributesByReference(List<string> representationIds, string schemaAttributeId, string value, CancellationToken cancellationToken)
    {
        var representationAttributes = await _scimDbContext.SCIMRepresentationAttributeLst.Include(s => s.SchemaAttribute).AsNoTracking()
            .Where(a => representationIds.Contains(a.RepresentationId) && a.SchemaAttributeId == schemaAttributeId && a.ValueString == value)
            .ToListAsync(cancellationToken);
        return representationAttributes;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributesByValue(string attrSchemaId, string value)
    {
        var result = await _scimDbContext.SCIMRepresentationAttributeLst.Include(a => a.SchemaAttribute).AsNoTracking()
            .Where(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueString == value)
            .ToListAsync();
        return result;
    }

    public async Task<List<SCIMRepresentationAttribute>> FindAttributesByValue(string attrSchemaId, int value)
    {
        var result = await _scimDbContext.SCIMRepresentationAttributeLst.Include(a => a.SchemaAttribute).AsNoTracking()
            .Where(a => a.SchemaAttribute.Id == attrSchemaId && a.ValueInteger == value)
            .ToListAsync();
        return result;
    }

    public async Task BulkInsert(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId, bool isReference = false)
    {
        scimRepresentationAttributes = scimRepresentationAttributes.Where(r => !string.IsNullOrWhiteSpace(r.RepresentationId)).ToList();
        foreach (var attr in scimRepresentationAttributes)
        {
            attr.SchemaAttributeId = attr.SchemaAttribute?.Id;
            attr.SchemaAttribute = null;
        }

        await _scimDbContext.AddRangeAsync(scimRepresentationAttributes);
    }

    public async Task BulkDelete(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, string currentRepresentationId, bool isReference = false)
    {
        if(_scimDbContext.Database.IsInMemory() || _options.IgnoreBulkOperation)
        {
            var ids = scimRepresentationAttributes.Select(a => a.Id).ToList();
            var attrs = await _scimDbContext.SCIMRepresentationAttributeLst.Where(a => ids.Contains(a.Id)).ToListAsync(); ;
            _scimDbContext.SCIMRepresentationAttributeLst.RemoveRange(attrs);
            await _scimDbContext.SaveChangesAsync();
            return;

        }
        scimRepresentationAttributes = scimRepresentationAttributes.Where(r => !string.IsNullOrWhiteSpace(r.RepresentationId)).ToList();
        var merged = LinqToDB.LinqExtensions.DeleteWhenMatched(
                        LinqToDB.LinqExtensions.On(
                            LinqToDB.LinqExtensions.Using(
                                LinqToDB.LinqExtensions.Merge(
                                    _scimDbContext.SCIMRepresentationAttributeLst.ToLinqToDBTable()),
                                    scimRepresentationAttributes
                                ),
                                (g1, g2) => g1.Id == g2.Id
                        ));
        await LinqToDB.LinqExtensions.MergeAsync(merged);
    }

    public async Task BulkUpdate(IEnumerable<SCIMRepresentationAttribute> scimRepresentationAttributes, bool isReference = false)
    {
        if(_scimDbContext.Database.IsInMemory() || _options.IgnoreBulkOperation)
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
            return;
        }

        scimRepresentationAttributes = scimRepresentationAttributes.Where(r => !string.IsNullOrWhiteSpace(r.RepresentationId)).ToList();
        foreach (var attr in scimRepresentationAttributes)
            attr.SchemaAttributeId = attr.SchemaAttribute?.Id;
        var merged = LinqToDB.LinqExtensions.UpdateWhenMatched(
                        LinqToDB.LinqExtensions.On(
                            LinqToDB.LinqExtensions.Using(
                                LinqToDB.LinqExtensions.Merge(
                                    _scimDbContext.SCIMRepresentationAttributeLst.ToLinqToDBTable()),
                                    scimRepresentationAttributes
                                ),
                                (g1, g2) => g1.Id == g2.Id
                        ), (target, source) => new SCIMRepresentationAttribute
                        {
                            ValueString = source.ValueString,
                            ValueDecimal = source.ValueDecimal,
                            ValueBoolean = source.ValueBoolean,
                            ValueBinary = source.ValueBinary,
                            ValueReference = source.ValueReference,
                            ValueInteger = source.ValueInteger,
                            ValueDateTime = source.ValueDateTime,
                            ComputedValueIndex = source.ComputedValueIndex
                        });
        await LinqToDB.LinqExtensions.MergeAsync(merged);
    }

    public async Task<ITransaction> StartTransaction(CancellationToken token)
    {
        var inMemory = _scimDbContext.Database.IsInMemory();
        IDbContextTransaction transaction = null;
        if(!inMemory) transaction = await _scimDbContext.Database.BeginTransactionAsync(token);
        return new EFTransaction(_scimDbContext, transaction);
    }

    private void ResolveChildren(IQueryable<SCIMRepresentationAttribute> representationAttributes, string parentId, List<SCIMRepresentationAttribute> children)
    {
        var filteredAttributes = representationAttributes.Where(a => a.ParentAttributeId == parentId);
        children.AddRange(filteredAttributes);
        foreach (var fAttr in filteredAttributes) ResolveChildren(representationAttributes, fAttr.Id, children);
    }
}
