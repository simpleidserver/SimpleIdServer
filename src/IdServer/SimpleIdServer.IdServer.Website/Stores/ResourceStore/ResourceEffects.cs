// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.ResourceStore
{
    public class ResourceEffects
    {
        private readonly IScopeRepository _scopeRepository;

        public ResourceEffects(IScopeRepository scopeRepository)
        {
            _scopeRepository = scopeRepository;
        }


        [EffectMethod]
        public async Task Handle(SearchResourcesAction action, IDispatcher dispatcher)
        {
            IQueryable<Scope> query = _scopeRepository.Query().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var scopes = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchResourcesSuccessAction { Scopes = scopes });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedResourcesAction action, IDispatcher dispatcher)
        {
            var scopes = await _scopeRepository.Query().Where(c => action.ResourceNames.Contains(c.Name)).ToListAsync(CancellationToken.None);
            _scopeRepository.DeleteRange(scopes);
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedResourcesSuccessAction { ResourceNames = action.ResourceNames });
        }

        [EffectMethod]
        public async Task Handle(AddIdentityResourceAction action, IDispatcher dispatcher)
        {
            if(await _scopeRepository.Query().AsNoTracking().AnyAsync(s => s.Name == action.Name, CancellationToken.None))
            {
                dispatcher.Dispatch(new AddResourceFailureAction { Name = action.Name, ErrorMessage = string.Format(Global.ResourceAlreadyExists, action.Name) });
                return;
            }

            var newScope = new Scope
            {
                Name = action.Name,
                Description = action.Description,
                IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            _scopeRepository.Add(newScope);
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddResourceSuccessAction { Name = action.Name, Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp });
        }
    }

    public class SearchResourcesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchResourcesSuccessAction
    {
        public IEnumerable<Scope> Scopes { get; set; } = new List<Scope>();
    }

    public class ToggleResourceSelectionAction
    {
        public bool IsSelected { get; set; }
        public string ResourceName { get; set; } = null!;
    }

    public class ToggleAllResourceSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedResourcesAction
    {
        public ICollection<string> ResourceNames { get; set; } = new List<string>();
    }

    public class RemoveSelectedResourcesSuccessAction
    {
        public ICollection<string> ResourceNames { get; set; } = new List<string>();
    }

    public class AddIdentityResourceAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; }
    }

    public class AddResourceSuccessAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; }
    }

    public class AddResourceFailureAction
    {
        public string Name { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }
}