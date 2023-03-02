// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    public class IdProviderEffects
    {
        private readonly IAuthenticationSchemeProviderRepository _repository;
        private readonly IAuthenticationSchemeProviderDefinitionRepository _repositoryDef;

        public IdProviderEffects(IAuthenticationSchemeProviderRepository repository, IAuthenticationSchemeProviderDefinitionRepository repositoryDef)
        {
            _repository = repository;
            _repositoryDef = repositoryDef;
        }

        [EffectMethod]
        public async Task Handle(SearchIdProvidersAction action, IDispatcher dispatcher)
        {
            IQueryable<AuthenticationSchemeProvider> query = _repository.Query().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var idProviders = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchIdProvidersSuccessAction { IdProviders = idProviders });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedIdProvidersAction action, IDispatcher dispatcher)
        {
            var idProviders = await _repository.Query().Include(p => p.Properties).Where(p => action.Ids.Contains(p.Name)).ToListAsync();
            _repository.RemoveRange(idProviders);
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedIdProvidersSuccessAction { Ids = action.Ids });
        }

        [EffectMethod]
        public async Task Handle(GetIdProviderAction action, IDispatcher dispatcher)
        {
            var idProvider = await _repository.Query().Include(i=> i.Properties).AsNoTracking().SingleOrDefaultAsync(p => p.Name == action.Id);
            if(idProvider == null)
            {
                dispatcher.Dispatch(new GetIdProviderFailureAction { ErrorMessage = string.Format(Global.UnknownIdProvider, action.Id) });
                return;
            }

            dispatcher.Dispatch(new GetIdProviderSuccessAction { IdProvider = idProvider });
        }

        [EffectMethod]
        public async Task Handle(GetIdProviderDefsAction action, IDispatcher dispatcher)
        {
            var idProviderDefs = await _repositoryDef.Query().Include(d => d.Properties).AsNoTracking().ToListAsync();
            dispatcher.Dispatch(new GetIdProviderDefsSuccessAction { AuthProviderDefinitions = idProviderDefs });
        }
    }

    public class SearchIdProvidersAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public string? ScopeName { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchIdProvidersSuccessAction
    {
        public ICollection<AuthenticationSchemeProvider> IdProviders { get; set; }
    }

    public class RemoveSelectedIdProvidersAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class RemoveSelectedIdProvidersSuccessAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class GetIdProviderAction
    {
        public string Id { get; set; }
    }

    public class GetIdProviderFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class GetIdProviderSuccessAction
    {
        public AuthenticationSchemeProvider IdProvider { get; set; }
    }

    public class ToggleIdProviderSelectionAction
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllIdProvidersSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class GetIdProviderDefsAction
    {

    }

    public class GetIdProviderDefsSuccessAction
    {
        public IEnumerable<AuthenticationSchemeProviderDefinition> AuthProviderDefinitions { get; set; }
    }
}
