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
            var idProvider = await _repository.Query().Include(i => i.Properties)
                .Include(i => i.Mappers)
                .Include(i => i.AuthSchemeProviderDefinition).ThenInclude(d => d.Properties)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Name == action.Id);
            if (idProvider == null)
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

        [EffectMethod]
        public async Task Handle(AddIdProviderAction action, IDispatcher dispatcher)
        {
            if (await _repository.Query().AsNoTracking().AnyAsync(r => r.Name == action.Name))
            {
                dispatcher.Dispatch(new AddIdProviderFailureAction { ErrorMessage = string.Format(Global.IdProviderExists, action.Name) });
                return;
            }

            var idProviderDef = await _repositoryDef.Query().SingleAsync(d => d.Name == action.IdProviderTypeName);
            var idProvider = new AuthenticationSchemeProvider
            {
                Name = action.Name,
                Description = action.Description,
                DisplayName = action.DisplayName,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Properties = action.Properties.ToList(),
                AuthSchemeProviderDefinition = idProviderDef,
                Mappers = Constants.GetDefaultIdProviderMappers()
            };
            _repository.Add(idProvider);
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddIdProviderSuccessAction { Name = action.Name, Description = action.Description, Properties = action.Properties, DisplayName = action.DisplayName });
        }

        [EffectMethod]
        public async Task Handle(UpdateIdProviderDetailsAction action, IDispatcher dispatcher)
        {
            var idProvider = await _repository.Query().SingleAsync(a => a.Name == action.Name);
            idProvider.Description = action.Description;
            idProvider.DisplayName = action.DisplayName;
            idProvider.UpdateDateTime = DateTime.UtcNow;
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateIdProviderDetailsSuccessAction { Description = action.Description, DisplayName = action.DisplayName, Name = action.Name });
        }

        [EffectMethod]
        public async Task Handle(UpdateAuthenticationSchemeProviderPropertiesAction action, IDispatcher dispatcher)
        {
            var idProvider = await _repository.Query().Include(p => p.Properties).SingleAsync(a => a.Name == action.Name);
            idProvider.Properties.Clear();
            foreach (var property in action.Properties)
                idProvider.Properties.Add(new AuthenticationSchemeProviderProperty
                {
                    PropertyName = property.PropertyName,
                    Value = property.Value
                });
            idProvider.UpdateDateTime = DateTime.UtcNow;
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateAuthenticationSchemeProviderPropertiesSuccessAction { Name = action.Name, Properties = action.Properties });
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

    public class AddIdProviderAction
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string IdProviderTypeName { get; set; } = null!;
        public string? Description { get; set; } = null;
        public IEnumerable<AuthenticationSchemeProviderProperty> Properties { get; set; }
    }

    public class AddIdProviderFailureAction
    {
        public string ErrorMessage { get; set; } = null!;
    }

    public class AddIdProviderSuccessAction
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Description { get; set; } = null;
        public IEnumerable<AuthenticationSchemeProviderProperty> Properties { get; set; }
    }

    public class UpdateIdProviderDetailsAction
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Description { get; set; }
    }

    public class UpdateIdProviderDetailsSuccessAction
    {
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Description { get; set; }
    }

    public class UpdateAuthenticationSchemeProviderPropertiesAction
    {
        public string Name { get; set; } = null!;
        public IEnumerable<AuthenticationSchemeProviderProperty> Properties { get; set; } = new List<AuthenticationSchemeProviderProperty>();
    }

    public class UpdateAuthenticationSchemeProviderPropertiesSuccessAction
    {
        public string Name { get; set; } = null!;
        public IEnumerable<AuthenticationSchemeProviderProperty> Properties { get; set; } = new List<AuthenticationSchemeProviderProperty>();
    }

    public class RemoveSelectedAuthenticationSchemeProviderMappersAction
    {
        public string Name { get; set; } = null!;
        public IEnumerable<string> MapperIds { get; set; } = new List<string>();
    }

    public class ToggleAuthenticationSchemeProviderMapperAction
    {
        public string MapperId { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllAuthenticationSchemeProviderSelectionAction
    {
        public bool IsSelected { get; set; }
    }
}
