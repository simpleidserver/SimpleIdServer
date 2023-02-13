// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.ApiResourceStore
{
    public class ApiResourceEffects
    {
        private readonly IApiResourceRepository _apiResourceRepository;
        private readonly IScopeRepository _scopeRepository;

        public ApiResourceEffects(IApiResourceRepository apiResourceRepository, IScopeRepository scopeRepository)
        {
            _apiResourceRepository = apiResourceRepository;
            _scopeRepository = scopeRepository;
        }

        [EffectMethod]
        public async Task Handle(SearchApiResourcesAction action, IDispatcher dispatcher)
        {
            IQueryable<ApiResource> query = _apiResourceRepository.Query().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var apiResources = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            var selectedResources = new List<string>();
            if(!string.IsNullOrWhiteSpace(action.ScopeName))
            {
                var scope = await _scopeRepository.Query().Include(s => s.ApiResources).AsNoTracking().SingleAsync(s => s.Name == action.ScopeName);
                selectedResources = scope.ApiResources.Select(r => r.Name).ToList();
            }

            dispatcher.Dispatch(new SearchApiResourcesSuccessAction { ApiResources = apiResources, SelectedApiResources = selectedResources });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(AddApiResourceAction action, IDispatcher dispatcher)
        {
            if (await _apiResourceRepository.Query().AsNoTracking().AnyAsync(r => r.Name == action.Name))
            {
                dispatcher.Dispatch(new AddApiResourceFailureAction { Name = action.Name, ErrorMessage = string.Format(Global.ApiResourceAlreadyExists, action.Name) });
                return;
            }

            var apiResource = new ApiResource
            {
                Name = action.Name,
                Description = action.Description,
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
            };
            _apiResourceRepository.Add(apiResource);
            await _apiResourceRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddApiResourceSuccessAction { Name = action.Name, Description = action.Description });
        }

        [EffectMethod]
        public async Task Handle(UpdateApiScopeResourcesAction action, IDispatcher dispatcher)
        {
            var scope = await _scopeRepository.Query().Include(s => s.ApiResources).SingleAsync(s => s.Name == action.Name, CancellationToken.None);
            var apiResources = await _apiResourceRepository.Query().Where(s => action.Resources.Contains(s.Name)).ToListAsync(CancellationToken.None);
            scope.ApiResources = apiResources;
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateApiScopeResourcesSuccessAction { Name = action.Name, Resources = action.Resources });
        }
    }

    public class SearchApiResourcesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public string? ScopeName { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchApiResourcesSuccessAction
    {
        public IEnumerable<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        public IEnumerable<string> SelectedApiResources { get; set; } = new List<string>();
    }

    public class AddApiResourceAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
    }

    public class AddApiResourceSuccessAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
    }

    public class AddApiResourceFailureAction
    {
        public string Name { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class ToggleApiResourceSelectionAction
    {
        public bool IsSelected { get; set; } = false;
        public string ResourceName { get; set; } = null!;
    }

    public class ToggleAllApiResourceSelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }

    public class UpdateApiScopeResourcesAction
    {
        public string Name { get; set; } = null!;
        public IEnumerable<string> Resources { get; set; } = new List<string>();
    }

    public class UpdateApiScopeResourcesSuccessAction
    {
        public string Name { get; set; } = null!;
        public IEnumerable<string> Resources { get; set; } = new List<string>();
    }
}
