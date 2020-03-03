// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System.Collections.Generic;
using System.Linq;
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

        public bool Add(SCIMRepresentation data)
        {
            var record = new SCIMRepresentationModel
            {
                Created = data.Created,
                ExternalId = data.ExternalId,
                LastModified = data.LastModified,
                Version = data.Version,
                ResourceType = data.ResourceType,
                Id = data.Id,
                Attributes = data.Attributes.Select(a => a.ToModel()).ToList(),
                Schemas = data.Schemas.Select(s => new SCIMRepresentationSchemaModel
                {
                    SCIMRepresentationId = data.Id,
                    SCIMSchemaId = s.Id
                }).ToList()
            };
            _scimDbContext.SCIMRepresentationLst.Add(record);
            return true;
        }

        public bool Delete(SCIMRepresentation data)
        {
            var result = _scimDbContext.SCIMRepresentationLst
                .Include(s => s.Attributes).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Attributes).ThenInclude(s => s.Values).ThenInclude(s => s.Values).ThenInclude(s => s.SchemaAttribute)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.Attributes)
                .Include(s => s.Schemas).ThenInclude(s => s.Schema).ThenInclude(s => s.SchemaExtensions)
                .FirstOrDefault(r => r.Id == data.Id);
            if (result == null)
            {
                return false;
            }

            var attrs = new List<SCIMRepresentationAttributeModel>();
             GetAllAttributes(result.Attributes, attrs);
            _scimDbContext.SCIMRepresentationAttributeLst.RemoveRange(attrs);
            _scimDbContext.SCIMRepresentationSchemaLst.RemoveRange(result.Schemas);
            _scimDbContext.SCIMRepresentationLst.Remove(result);
            return true;
        }

        public bool Update(SCIMRepresentation data)
        {
            if (!Delete(data))
            {
                return false;
            }

            Add(data);
            return true;
        }

        public Task<int> SaveChanges()
        {
            return _scimDbContext.SaveChangesAsync();
        }

        private static void GetAllAttributes(ICollection<SCIMRepresentationAttributeModel> attrs, List<SCIMRepresentationAttributeModel> result)
        {
            result.AddRange(attrs);
            foreach(var attr in attrs)
            {
                if (attr.Values != null && attr.Values.Any())
                {
                    GetAllAttributes(attr.Values, result);
                }
            }
        }

        public void Dispose() { }
    }
}
