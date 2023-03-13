// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
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
        private readonly DbContextOptions<StoreDbContext> _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public IdProviderEffects(IAuthenticationSchemeProviderRepository repository, IAuthenticationSchemeProviderDefinitionRepository repositoryDef, DbContextOptions<StoreDbContext> options, ProtectedSessionStorage sessionStorage)
        {
            _repository = repository;
            _repositoryDef = repositoryDef;
            _options = options;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchIdProvidersAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            IQueryable<AuthenticationSchemeProvider> query = _repository.Query().Include(p => p.Realms).Where(p => p.Realms.Any(r => r.Name == realm)).AsNoTracking();
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
            var realm = await GetRealm();
            var idProviders = await _repository.Query().Include(r => r.Realms).Include(p => p.Properties).Where(p => action.Ids.Contains(p.Name) && p.Realms.Any(r => r.Name == realm)).ToListAsync();
            _repository.RemoveRange(idProviders);
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedIdProvidersSuccessAction { Ids = action.Ids });
        }

        [EffectMethod]
        public async Task Handle(GetIdProviderAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var idProvider = await _repository.Query().Include(i => i.Properties)
                .Include(i => i.Mappers)
                .Include(i => i.Realms)
                .Include(i => i.AuthSchemeProviderDefinition).ThenInclude(d => d.Properties)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Name == action.Id && p.Realms.Any(r => r.Name == realm));
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
            if(string.IsNullOrWhiteSpace(action.Name))
            {
                dispatcher.Dispatch(new AddIdProviderFailureAction { ErrorMessage = Global.NameIsRequired });
                return;
            }

            var realm = await GetRealm();
            if (await _repository.Query().Include(r => r.Realms).AsNoTracking().AnyAsync(r => r.Name == action.Name && r.Realms.Any(rl => rl.Name == realm)))
            {
                dispatcher.Dispatch(new AddIdProviderFailureAction { ErrorMessage = string.Format(Global.IdProviderExists, action.Name) });
                return;
            }

            using (var dbContext = new StoreDbContext(_options))
            {
                var idProviderDef = await dbContext.AuthenticationSchemeProviderDefinitions.SingleAsync(d => d.Name == action.IdProviderTypeName);
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var idProvider = new AuthenticationSchemeProvider
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = action.Name,
                    Description = action.Description,
                    DisplayName = action.DisplayName,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow,
                    Properties = action.Properties.ToList(),
                    AuthSchemeProviderDefinition = idProviderDef,
                    Mappers = Constants.GetDefaultIdProviderMappers()
                };
                idProvider.Realms.Add(activeRealm);
                dbContext.Add(idProvider);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddIdProviderSuccessAction { Name = action.Name, Description = action.Description, Properties = action.Properties, DisplayName = action.DisplayName });
            }
        }

        [EffectMethod]
        public async Task Handle(UpdateIdProviderDetailsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var idProvider = await _repository.Query().Include(r => r.Realms).SingleAsync(a => a.Name == action.Name && a.Realms.Any(r => r.Name == realm));
            idProvider.Description = action.Description;
            idProvider.DisplayName = action.DisplayName;
            idProvider.UpdateDateTime = DateTime.UtcNow;
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateIdProviderDetailsSuccessAction { Description = action.Description, DisplayName = action.DisplayName, Name = action.Name });
        }

        [EffectMethod]
        public async Task Handle(UpdateAuthenticationSchemeProviderPropertiesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var idProvider = await _repository.Query().Include(r => r.Realms).Include(p => p.Properties).SingleAsync(a => a.Name == action.Name && a.Realms.Any(r => r.Name == realm));
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

        [EffectMethod]
        public async Task Handle(AddAuthenticationSchemeProviderMapperAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var idProvider = await _repository.Query().Include(r => r.Realms).Include(p => p.Mappers).SingleAsync(a => a.Name == action.IdProviderName && a.Realms.Any(r => r.Name == realm));
            var id = Guid.NewGuid().ToString();
            idProvider.Mappers.Add(new AuthenticationSchemeProviderMapper
            {
                Id = id,
                MapperType = action.MapperType,
                Name = action.Name,
                SourceClaimName = action.SourceClaimName,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty
            });
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddAuthenticationSchemeProviderMapperSuccessAction
            {
                Id = id,
                IdProviderName= action.IdProviderName,
                MapperType= action.MapperType,
                Name= action.Name,
                SourceClaimName= action.SourceClaimName,
                TargetUserAttribute= action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty
            });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedAuthenticationSchemeProviderMappersAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var idProvider = await _repository.Query().Include(p => p.Realms).Include(p => p.Mappers).SingleAsync(a => a.Name == action.Name && a.Realms.Any(r => r.Name == realm));
            idProvider.Mappers = idProvider.Mappers.Where(m => !action.MapperIds.Contains(m.Id)).ToList();
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedAuthenticationSchemeProviderMappersSuccessAction
            {
                Name = action.Name,
                MapperIds = action.MapperIds
            });
        }

        [EffectMethod]
        public async Task Handle(UpdateAuthenticationSchemeProviderMapperAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var idProvider = await _repository.Query().Include(p => p.Realms).Include(p => p.Mappers).SingleAsync(a => a.Name == action.IdProviderName && a.Realms.Any(r => r.Name == realm));
            var mapper = idProvider.Mappers.First(m => m.Id == action.Id);
            mapper.Name = action.Name;
            mapper.SourceClaimName = action.SourceClaimName;
            mapper.TargetUserAttribute = action.TargetUserAttribute;
            mapper.TargetUserProperty = action.TargetUserProperty;
            await _repository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateAuthenticationSchemeProviderMapperSuccessAction
            {
                Id = action.Id,
                Name = action.Name,
                IdProviderName = action.IdProviderName,
                SourceClaimName = action.SourceClaimName,
                TargetUserAttribute = action.TargetUserAttribute,
                TargetUserProperty = action.TargetUserProperty
            });
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
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

    public class RemoveSelectedAuthenticationSchemeProviderMappersSuccessAction
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

    public class AddAuthenticationSchemeProviderMapperAction
    {
        public string IdProviderName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public AuthenticationSchemeProviderMapperTypes MapperType { get; set; }
        public string? SourceClaimName { get; set; } = null;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
    }

    public class AddAuthenticationSchemeProviderMapperSuccessAction
    {
        public string IdProviderName { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public AuthenticationSchemeProviderMapperTypes MapperType { get; set; }
        public string? SourceClaimName { get; set; } = null;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
    }

    public class UpdateAuthenticationSchemeProviderMapperAction
    {
        public string IdProviderName { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? SourceClaimName { get; set; } = null;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
    }

    public class UpdateAuthenticationSchemeProviderMapperSuccessAction
    {
        public string IdProviderName { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? SourceClaimName { get; set; } = null;
        public string? TargetUserAttribute { get; set; } = null;
        public string? TargetUserProperty { get; set; } = null;
    }
}
