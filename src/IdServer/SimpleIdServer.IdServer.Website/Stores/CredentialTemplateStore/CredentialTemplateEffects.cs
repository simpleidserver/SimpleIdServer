// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.CredentialTemplateStore
{
    public class CredentialTemplateEffects
    {
        private readonly ICredentialTemplateRepository _credentialTemplateRepository;
        private readonly ProtectedSessionStorage _sessionStorage;

        public CredentialTemplateEffects(ICredentialTemplateRepository credentialTemplateRepository, ProtectedSessionStorage sessionStorage)
        {
            _credentialTemplateRepository = credentialTemplateRepository;
            _sessionStorage = sessionStorage;
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
            dispatcher.Dispatch(new RemoveSelectedCredentialTemplatesSuccessAction { });

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
}
