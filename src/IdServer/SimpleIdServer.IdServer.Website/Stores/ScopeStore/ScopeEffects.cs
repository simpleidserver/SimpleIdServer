// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using SimpleIdServer.IdServer.Website.Stores.RealmStore;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.ScopeStore
{
    public class ScopeEffects
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly DbContextOptions<StoreDbContext> _options;

        public ScopeEffects(IScopeRepository scopeRepository, ProtectedSessionStorage sessionStorage, DbContextOptions<StoreDbContext> options)
        {
            _scopeRepository = scopeRepository;
            _sessionStorage = sessionStorage;
            _options = options;
        }


        [EffectMethod]
        public async Task Handle(SearchScopesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            IQueryable<Scope> query = _scopeRepository.Query().Include(s => s.Realms).Where(s => s.Realms.Any(r => r.Name == realm)).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));
            else
                query = query.OrderBy(a => a.Name);

            if (!string.IsNullOrWhiteSpace(action.ClientType))
            {
                if (action.ClientType == SimpleIdServer.IdServer.WsFederation.WsFederationConstants.CLIENT_TYPE)
                    query = query.Where(q => q.Protocol == ScopeProtocols.SAML);
                else
                    query = query.Where(q => q.Protocol == ScopeProtocols.OAUTH || q.Protocol == ScopeProtocols.OPENID);
            }

            var scopes = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchScopesSuccessAction { Scopes = scopes });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedScopesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var scopes = await _scopeRepository.Query().Include(s => s.Realms).Where(c => action.ScopeNames.Contains(c.Name) && c.Realms.Any(r => r.Name == realm)).ToListAsync(CancellationToken.None);
            _scopeRepository.DeleteRange(scopes);
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedScopesSuccessAction { ScopeNames = action.ScopeNames });
        }

        [EffectMethod]
        public async Task Handle(AddIdentityScopeAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            if (await _scopeRepository.Query().Include(s => s.Realms).AsNoTracking().AnyAsync(s => s.Name == action.Name && s.Realms.Any(r => r.Name == realm), CancellationToken.None))
            {
                dispatcher.Dispatch(new AddScopeFailureAction { Name = action.Name, ErrorMessage = string.Format(Global.ScopeAlreadyExists, action.Name) });
                return;
            }

            using (var dbContext = new StoreDbContext(_options))
            {
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var newScope = new Scope
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = action.Name,
                    Description = action.Description,
                    Type = ScopeTypes.IDENTITY,
                    IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp,
                    Protocol = action.Protocol,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                };
                newScope.Realms.Add(activeRealm);
                dbContext.Scopes.Add(newScope);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddScopeSuccessAction { Name = action.Name, Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp, Protocol = action.Protocol, Type = ScopeTypes.IDENTITY });
            }
        }

        [EffectMethod]
        public async Task Handle(AddApiScopeAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            if (await _scopeRepository.Query().Include(s => s.Realms).AsNoTracking().AnyAsync(s => s.Name == action.Name && s.Realms.Any(r => r.Name == realm), CancellationToken.None))
            {
                dispatcher.Dispatch(new AddScopeFailureAction { Name = action.Name, ErrorMessage = string.Format(Global.ScopeAlreadyExists, action.Name) });
                return;
            }

            using (var dbContext = new StoreDbContext(_options))
            {
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var newScope = new Scope
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = action.Name,
                    Description = action.Description,
                    Type = ScopeTypes.APIRESOURCE,
                    IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp,
                    Protocol = ScopeProtocols.OAUTH,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                };
                newScope.Realms.Add(activeRealm);
                dbContext.Scopes.Add(newScope);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddScopeSuccessAction { Name = action.Name, Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp, Protocol = ScopeProtocols.OAUTH, Type = ScopeTypes.APIRESOURCE });
            }
        }

        [EffectMethod]
        public async Task Handle(GetScopeAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var scope = await _scopeRepository.Query().Include(s => s.Realms).Include(s => s.ClaimMappers).AsNoTracking().SingleOrDefaultAsync(s => s.Name == action.ScopeName && s.Realms.Any(r => r.Name == realm), CancellationToken.None);
            if (scope == null)
            {
                dispatcher.Dispatch(new GetScopeFailureAction { ScopeName = action.ScopeName, ErrorMessage = string.Format(Global.UnknownResource, action.ScopeName) });
                return;
            }

            dispatcher.Dispatch(new GetScopeSuccessAction { Scope = scope });
        }

        [EffectMethod]
        public async Task Handle(UpdateScopeAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var scope = await _scopeRepository.Query().Include(s => s.Realms).SingleAsync(s => s.Name == action.ScopeName && s.Realms.Any(r => r.Name == realm), CancellationToken.None);
            scope.Description = action.Description;
            scope.IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp;
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateScopeSuccessAction { Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp, ScopeName = action.ScopeName });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedScopeMappersAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var scope = await _scopeRepository.Query().Include(s => s.Realms).Include(s => s.ClaimMappers).SingleAsync(s => s.Name == action.ScopeName && s.Realms.Any(r => r.Name == realm), CancellationToken.None);
            scope.ClaimMappers = scope.ClaimMappers.Where(m => !action.ScopeMapperIds.Contains(m.Id)).ToList();
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedScopeMappersSuccessAction { ScopeMapperIds = action.ScopeMapperIds, ScopeName = action.ScopeName });
        }

        [EffectMethod]
        public async Task Handle(AddScopeClaimMapperAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var scope = await _scopeRepository.Query().Include(s => s.Realms).Include(s => s.ClaimMappers).SingleAsync(s => s.Name == action.ScopeName && s.Realms.Any(r => r.Name == realm), CancellationToken.None);
            if (scope.ClaimMappers.Any(m => m.Name == action.ClaimMapper.Name))
            {
                dispatcher.Dispatch(new AddScopeClaimMapperFailureAction { ScopeName = action.ScopeName, ErrorMessage = Global.ScopeClaimMapperNameMustBeUnique });
                return;
            }

            if (!string.IsNullOrWhiteSpace(action.ClaimMapper.TokenClaimName) && scope.ClaimMappers.Any(m => m.TokenClaimName == action.ClaimMapper.TokenClaimName))
            {
                dispatcher.Dispatch(new AddScopeClaimMapperFailureAction { ScopeName = action.ScopeName, ErrorMessage = Global.ScopeClaimMapperTokenClaimNameMustBeUnique });
                return;
            }

            if (!string.IsNullOrWhiteSpace(action.ClaimMapper.SAMLAttributeName) && scope.ClaimMappers.Any(m => m.SAMLAttributeName == action.ClaimMapper.SAMLAttributeName))
            {
                dispatcher.Dispatch(new AddScopeClaimMapperFailureAction { ScopeName = action.ScopeName, ErrorMessage = Global.ScopeClaimMapperSAMLAttributeNameMustBeUnique });
                return;
            }

            scope.ClaimMappers.Add(action.ClaimMapper);
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddScopeClaimMapperSuccessAction { ClaimMapper = action.ClaimMapper, ScopeName = action.ScopeName });
        }

        [EffectMethod]
        public async Task Handle(UpdateScopeClaimMapperAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            var scope = await _scopeRepository.Query().Include(s => s.Realms).Include(s => s.ClaimMappers).SingleAsync(s => s.Name == action.ScopeName && s.Realms.Any(r => r.Name == realm), CancellationToken.None);
            if (!string.IsNullOrWhiteSpace(action.ClaimMapper.TokenClaimName) && scope.ClaimMappers.Any(m => m.TokenClaimName == action.ClaimMapper.TokenClaimName && m.Name != action.ClaimMapper.Name))
            {
                dispatcher.Dispatch(new UpdateScopeClaimMapperFailureAction { ScopeName = action.ScopeName, ErrorMessage = Global.ScopeClaimMapperTokenClaimNameMustBeUnique });
                return;
            }

            if (!string.IsNullOrWhiteSpace(action.ClaimMapper.SAMLAttributeName) && scope.ClaimMappers.Any(m => m.SAMLAttributeName == action.ClaimMapper.SAMLAttributeName && m.Name != action.ClaimMapper.Name))
            {
                dispatcher.Dispatch(new UpdateScopeClaimMapperFailureAction { ScopeName = action.ScopeName, ErrorMessage = Global.ScopeClaimMapperSAMLAttributeNameMustBeUnique });
                return;
            }

            var mapper = scope.ClaimMappers.Single(m => m.Name == action.ClaimMapper.Name);
            mapper.UserAttributeName = action.ClaimMapper.UserAttributeName;
            mapper.UserAttributeStreetName = action.ClaimMapper.UserAttributeStreetName;
            mapper.UserAttributeLocalityName = action.ClaimMapper.UserAttributeLocalityName;
            mapper.UserAttributeRegionName = action.ClaimMapper.UserAttributeRegionName;
            mapper.UserAttributePostalCodeName = action.ClaimMapper.UserAttributePostalCodeName;
            mapper.UserAttributeCountryName = action.ClaimMapper.UserAttributeCountryName;
            mapper.UserAttributeFormattedName = action.ClaimMapper.UserAttributeFormattedName;
            mapper.UserPropertyName = action.ClaimMapper.UserPropertyName;
            mapper.TokenClaimName = action.ClaimMapper.TokenClaimName;
            mapper.SAMLAttributeName = action.ClaimMapper.SAMLAttributeName;
            mapper.TokenClaimJsonType = action.ClaimMapper.TokenClaimJsonType;
            mapper.IsMultiValued = action.ClaimMapper.IsMultiValued;
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateScopeClaimMapperSuccessAction { ClaimMapper = action.ClaimMapper, ScopeName = action.ScopeName });
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchScopesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
        public string? ClientType { get; set; } = null;
    }

    public class SearchScopesSuccessAction
    {
        public IEnumerable<Scope> Scopes { get; set; } = new List<Scope>();
    }

    public class ToggleScopeSelectionAction
    {
        public bool IsSelected { get; set; }
        public string ScopeName { get; set; } = null!;
    }

    public class ToggleAllScopeSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedScopesAction
    {
        public ICollection<string> ScopeNames { get; set; } = new List<string>();
    }

    public class RemoveSelectedScopesSuccessAction
    {
        public ICollection<string> ScopeNames { get; set; } = new List<string>();
    }

    public class AddIdentityScopeAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public ScopeProtocols Protocol { get; set; }
        public bool IsExposedInConfigurationEdp { get; set; }
    }

    public class AddScopeSuccessAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; }
        public ScopeProtocols Protocol { get; set; }
        public ScopeTypes Type { get; set; }
    }

    public class AddScopeFailureAction
    {
        public string Name { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class GetScopeAction
    {
        public string ScopeName { get; set; } = null!;
    }

    public class GetScopeSuccessAction
    {
        public Scope Scope { get; set; } = null!;
    }

    public class GetScopeFailureAction
    {
        public string ScopeName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class UpdateScopeAction
    {
        public string ScopeName { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; } = false;
    }

    public class UpdateScopeSuccessAction
    {
        public string ScopeName { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; } = false;
    }

    public class ToggleScopeMapperSelectionAction
    {
        public bool IsSelected { get; set; }
        public string ScopeMapperId { get; set; } = null!;
    }

    public class ToggleAllScopeMapperSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedScopeMappersAction
    {
        public string ScopeName { get; set; } = null!;
        public ICollection<string> ScopeMapperIds { get; set; } = new List<string>();
    }

    public class RemoveSelectedScopeMappersSuccessAction
    {
        public string ScopeName { get; set; } = null!;
        public ICollection<string> ScopeMapperIds { get; set; } = new List<string>();
    }

    public class AddScopeClaimMapperAction
    {
        public string ScopeName { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class AddScopeClaimMapperSuccessAction
    {
        public string ScopeName { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class AddScopeClaimMapperFailureAction
    {
        public string ScopeName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class UpdateScopeClaimMapperAction
    {
        public string ScopeName { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class UpdateScopeClaimMapperSuccessAction
    {
        public string ScopeName { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class UpdateScopeClaimMapperFailureAction
    {
        public string ScopeName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class AddApiScopeAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; }
    }
}