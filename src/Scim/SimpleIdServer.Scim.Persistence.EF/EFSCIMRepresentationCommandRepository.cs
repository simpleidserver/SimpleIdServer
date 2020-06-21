// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMRepresentationCommandRepository : ISCIMRepresentationCommandRepository
    {
        private readonly SCIMDbContext _scimDbContext;

        public EFSCIMRepresentationCommandRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<ITransaction> StartTransaction(CancellationToken token)
        {
            var transaction = await _scimDbContext.Database.BeginTransactionAsync(token);
            return new EFTransaction(_scimDbContext, transaction);
        }

        public Task<bool> Add(SCIMRepresentation data, CancellationToken token)
        {
            var record = new SCIMRepresentationModel
            {
                Created = data.Created,
                ExternalId = data.ExternalId,
                LastModified = data.LastModified,
                Version = data.Version,
                ResourceType = data.ResourceType,
                Id = data.Id,
                Attributes = data.Attributes.Select(a => a.ToModel(data.Id)).ToList(),
                Schemas = data.Schemas.Select(s => new SCIMRepresentationSchemaModel
                {
                    SCIMRepresentationId = data.Id,
                    SCIMSchemaId = s.Id
                }).ToList()
            };
            _scimDbContext.SCIMRepresentationLst.Add(record);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
        {
            var result = _scimDbContext.SCIMRepresentationLst
                .Include(s => s.Attributes).ThenInclude(s => s.Values)
                .Include(s => s.Attributes).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Children).ThenInclude(s => s.Values)
                .Include(s => s.Attributes).ThenInclude(s => s.Children).ThenInclude(s => s.Children)
                .Include(s => s.Attributes).ThenInclude(s => s.Children).ThenInclude(s => s.Children).ThenInclude(s => s.Values)
                .Include(s => s.Attributes).ThenInclude(s => s.Children).ThenInclude(s => s.Children).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.Attributes)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.SchemaExtensions)
                .FirstOrDefault(r => r.Id == data.Id);
            if (result == null)
            {
                return Task.FromResult(false);
            }

            var attrs = new List<SCIMRepresentationAttributeModel>();
             GetAllAttributes(result.Attributes, attrs);
            _scimDbContext.SCIMRepresentationAttributeLst.RemoveRange(attrs);
            foreach(var attr in attrs)
            {
                _scimDbContext.SCIMRepresentationAttributeValueLst.RemoveRange(attr.Values);
            }

            _scimDbContext.SCIMRepresentationSchemaLst.RemoveRange(result.Schemas);
            _scimDbContext.SCIMRepresentationLst.Remove(result);
            return Task.FromResult(true);
        }

        public async Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            if (!await Delete(data, token))
            {
                return false;
            }

            await Add(data, token);
            return true;
        }

        private static void GetAllAttributes(ICollection<SCIMRepresentationAttributeModel> attrs, List<SCIMRepresentationAttributeModel> result)
        {
            result.AddRange(attrs);
            foreach(var attr in attrs)
            {
                if (attr.Children != null && attr.Children.Any())
                {
                    GetAllAttributes(attr.Children, result);
                }
            }
        }
    }
}
