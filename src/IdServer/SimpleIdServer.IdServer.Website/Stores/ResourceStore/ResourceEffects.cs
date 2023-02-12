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
                Protocol = action.Protocol,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            _scopeRepository.Add(newScope);
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddResourceSuccessAction { Name = action.Name, Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp, Protocol = action.Protocol });
        }

        [EffectMethod]
        public async Task Handle(GetResourceAction action, IDispatcher dispatcher)
        {
            var scope = await _scopeRepository.Query().Include(s => s.ClaimMappers).AsNoTracking().SingleOrDefaultAsync(s => s.Name == action.ResourceName, CancellationToken.None);
            if (scope == null)
            {
                dispatcher.Dispatch(new GetResourceFailureAction { ResourceName = action.ResourceName, ErrorMessage = string.Format(Global.UnknownResource, action.ResourceName) });
                return;
            }

            dispatcher.Dispatch(new GetResourceSuccessAction { Resource = scope });
        }

        [EffectMethod]
        public async Task Handle(UpdateResourceAction action, IDispatcher dispatcher)
        {
            var scope = await _scopeRepository.Query().SingleAsync(s => s.Name == action.ResourceName, CancellationToken.None);
            scope.Description = action.Description;
            scope.IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp;
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateResourceSuccessAction { Description = action.Description, IsExposedInConfigurationEdp = action.IsExposedInConfigurationEdp, ResourceName = action.ResourceName });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedResourceMappersAction action, IDispatcher dispatcher)
        {
            var scope = await _scopeRepository.Query().Include(s => s.ClaimMappers).SingleAsync(s => s.Name == action.ResourceName, CancellationToken.None);
            scope.ClaimMappers = scope.ClaimMappers.Where(m => !action.ResourceMapperIds.Contains(m.Id)).ToList();
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedResourceMappersSuccessAction { ResourceMapperIds = action.ResourceMapperIds, ResourceName = action.ResourceName });
        }

        [EffectMethod]
        public async Task Handle(AddResourceClaimMapperAction action, IDispatcher dispatcher)
        {
            var scope = await _scopeRepository.Query().Include(s => s.ClaimMappers).SingleAsync(s => s.Name == action.ResourceName, CancellationToken.None);
            if (scope.ClaimMappers.Any(m => m.Name == action.ClaimMapper.Name))
            {
                dispatcher.Dispatch(new AddResourceClaimMapperFailureAction { ResourceName = action.ResourceName, ErrorMessage = Global.ResourceClaimMapperNameMustBeUnique });
                return;
            }

            if (!string.IsNullOrWhiteSpace(action.ClaimMapper.TokenClaimName) && scope.ClaimMappers.Any(m => m.TokenClaimName == action.ClaimMapper.TokenClaimName))
            {
                dispatcher.Dispatch(new AddResourceClaimMapperFailureAction { ResourceName = action.ResourceName, ErrorMessage = Global.ResourceClaimMapperTokenClaimNameMustBeUnique });
                return;
            }

            if (!string.IsNullOrWhiteSpace(action.ClaimMapper.SAMLAttributeName) && scope.ClaimMappers.Any(m => m.SAMLAttributeName == action.ClaimMapper.SAMLAttributeName))
            {
                dispatcher.Dispatch(new AddResourceClaimMapperFailureAction { ResourceName = action.ResourceName, ErrorMessage = Global.ResourceClaimMapperSAMLAttributeNameMustBeUnique });
                return;
            }

            scope.ClaimMappers.Add(action.ClaimMapper);
            await _scopeRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddResourceClaimMapperSuccessAction { ClaimMapper = action.ClaimMapper, ResourceName = action.ResourceName });
        }

        [EffectMethod]
        public async Task Handle(UpdateResourceClaimMapperAction action, IDispatcher dispatcher)
        {
            var scope = await _scopeRepository.Query().Include(s => s.ClaimMappers).SingleAsync(s => s.Name == action.ResourceName, CancellationToken.None);
            if (!string.IsNullOrWhiteSpace(action.ClaimMapper.TokenClaimName) && scope.ClaimMappers.Any(m => m.TokenClaimName == action.ClaimMapper.TokenClaimName && m.Name != action.ClaimMapper.Name))
            {
                dispatcher.Dispatch(new UpdateResourceClaimMapperFailureAction { ResourceName = action.ResourceName, ErrorMessage = Global.ResourceClaimMapperTokenClaimNameMustBeUnique });
                return;
            }

            if (!string.IsNullOrWhiteSpace(action.ClaimMapper.SAMLAttributeName) && scope.ClaimMappers.Any(m => m.SAMLAttributeName == action.ClaimMapper.SAMLAttributeName && m.Name != action.ClaimMapper.Name))
            {
                dispatcher.Dispatch(new UpdateResourceClaimMapperFailureAction { ResourceName = action.ResourceName, ErrorMessage = Global.ResourceClaimMapperSAMLAttributeNameMustBeUnique });
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
            dispatcher.Dispatch(new UpdateResourceClaimMapperSuccessAction { ClaimMapper = action.ClaimMapper, ResourceName = action.ResourceName });
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
        public ScopeProtocols Protocol { get; set; }
        public bool IsExposedInConfigurationEdp { get; set; }
    }

    public class AddResourceSuccessAction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; }
        public ScopeProtocols Protocol { get; set; }
    }

    public class AddResourceFailureAction
    {
        public string Name { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class GetResourceAction
    {
        public string ResourceName { get; set; } = null!;
    }

    public class GetResourceSuccessAction
    {
        public Scope Resource { get; set; } = null!;
    }

    public class GetResourceFailureAction
    {
        public string ResourceName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class UpdateResourceAction
    {
        public string ResourceName { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; } = false;
    }

    public class UpdateResourceSuccessAction
    {
        public string ResourceName { get; set; } = null!;
        public string? Description { get; set; } = null;
        public bool IsExposedInConfigurationEdp { get; set; } = false;
    }

    public class ToggleResourceMapperSelectionAction
    {
        public bool IsSelected { get; set; }
        public string ResourceMapperId { get; set; } = null!;
    }

    public class ToggleAllResourceMapperSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedResourceMappersAction
    {
        public string ResourceName { get; set; } = null!;
        public ICollection<string> ResourceMapperIds { get; set; } = new List<string>();
    }

    public class RemoveSelectedResourceMappersSuccessAction
    {
        public string ResourceName { get; set; } = null!;
        public ICollection<string> ResourceMapperIds { get; set; } = new List<string>();
    }

    public class AddResourceClaimMapperAction
    {
        public string ResourceName { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class AddResourceClaimMapperSuccessAction
    {
        public string ResourceName { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class AddResourceClaimMapperFailureAction
    {
        public string ResourceName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class UpdateResourceClaimMapperAction
    {
        public string ResourceName { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class UpdateResourceClaimMapperSuccessAction
    {
        public string ResourceName { get; set; } = null!;
        public ScopeClaimMapper ClaimMapper { get; set; } = null!;
    }

    public class UpdateResourceClaimMapperFailureAction
    {
        public string ResourceName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }
}