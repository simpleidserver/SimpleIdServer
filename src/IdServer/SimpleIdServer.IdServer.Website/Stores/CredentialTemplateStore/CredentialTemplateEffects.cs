// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    public class CredentialTemplateEffects
    {
        private readonly ICredentialTemplateRepository _credentialTemplateRepository;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly DbContextOptions<StoreDbContext> _options;

        public CredentialTemplateEffects(ICredentialTemplateRepository credentialTemplateRepository, ProtectedSessionStorage sessionStorage, DbContextOptions<StoreDbContext> options)
        {
            _credentialTemplateRepository = credentialTemplateRepository;
            _sessionStorage = sessionStorage;
            _options = options;
        }

        [EffectMethod]
        public async Task Handle(SearchCredentialTemplatesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            IQueryable<CredentialTemplate> query = _credentialTemplateRepository.Query().Include(c => c.Parameters).Include(c => c.Realms).Include(c => c.DisplayLst).Where(c => c.Realms.Any(r => r.Name == realm)).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var nb = query.Count();
            var clients = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchCredentialTemplatesSuccessAction { CredentialTemplates = clients, Count = nb });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedCredentialTemplatesAction action, IDispatcher dispatcher)
        {
            var records = await _credentialTemplateRepository.Query().Where(c => action.CredentialTemplateIds.Contains(c.Id)).ToListAsync();
            _credentialTemplateRepository.Delete(records);
            await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedCredentialTemplatesSuccessAction { CredentialTemplateIds = action.CredentialTemplateIds });

        }

        [EffectMethod]
        public async Task Handle(AddW3CCredentialTemplateAction action, IDispatcher dispatcher)
        {
            var exists = await _credentialTemplateRepository.Query().Include(q => q.Parameters).AnyAsync(t => t.Format == Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials && t.Parameters.Any(p => p.Name == "type" && p.Value == action.Type));
            if(exists)
            {
                dispatcher.Dispatch(new AddCredentialTemplateErrorAction { ErrorMessage = Global.CredentialTemplateExists });
                return;
            }

            var realm = await GetRealm();
            using (var dbContext = new StoreDbContext(_options))
            {
                var existingRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var credentialTemplate = CredentialTemplateBuilder.NewW3CCredential(action.Name, action.LogoUrl, Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials, existingRealm).Build();
                dbContext.CredentialTemplates.Add(credentialTemplate);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddCredentialTemplateSuccessAction { Credential = credentialTemplate });
            }
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchCredentialTemplatesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchCredentialTemplatesSuccessAction
    {
        public IEnumerable<CredentialTemplate> CredentialTemplates { get; set; } = new List<CredentialTemplate>();
        public int Count { get; set; }
    }

    public class ToggleAllCredentialTemplatesAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleCredentialTemplateAction
    {
        public bool IsSelected { get; set; }
        public string CredentialTemplateId { get; set; }
    }

    public class RemoveSelectedCredentialTemplatesAction
    {
        public IEnumerable<string> CredentialTemplateIds { get; set; }
    }

    public class RemoveSelectedCredentialTemplatesSuccessAction
    {
        public IEnumerable<string> CredentialTemplateIds { get; set; }
    }

    public class AddW3CCredentialTemplateAction
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string Type { get; set; }
    }

    public class AddCredentialTemplateSuccessAction
    {
        public CredentialTemplate Credential { get; set; }
    }

    public class AddCredentialTemplateErrorAction
    {
        public string ErrorMessage { get; set; }
    }
}
