// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using SimpleIdServer.Vc.Builders;
using SimpleIdServer.Vc.Models;
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
            var clients = await query.ToListAsync(CancellationToken.None);
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
                var w3CCredentialTemplate = W3CCredentialTemplateBuilder.New(action.Name, action.LogoUrl, action.Type).Build();
                var credentialTemplate = new CredentialTemplate(w3CCredentialTemplate);
                credentialTemplate.Realms.Add(existingRealm);
                dbContext.CredentialTemplates.Add(credentialTemplate);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddCredentialTemplateSuccessAction { Credential = credentialTemplate });
            }
        }

        [EffectMethod]
        public async Task Handle(GetCredentialTemplateAction action, IDispatcher dispatcher)
        {
            var credentialTemplate = await _credentialTemplateRepository.Query().Include(c => c.DisplayLst).Include(c => c.Parameters).FirstAsync(c => c.TechnicalId == action.Id);
            dispatcher.Dispatch(new GetCredentialTemplateSuccessAction { CredentialTemplate = credentialTemplate });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedCredentialTemplateDisplayAction action, IDispatcher dispatcher)
        {
            var credentialTemplate = await _credentialTemplateRepository.Query().Include(c => c.DisplayLst).FirstAsync(c => c.TechnicalId == action.Id);
            credentialTemplate.DisplayLst = credentialTemplate.DisplayLst.Where(d => !action.DisplayIds.Contains(d.Id)).ToList();
            await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedCredentialTemplateDisplaySuccessAction { Id = action.Id, DisplayIds = action.DisplayIds });
        }

        [EffectMethod]
        public async Task Handle(AddCredentialTemplateDisplayAction action, IDispatcher dispatcher)
        {
            var credentialTemplate = await _credentialTemplateRepository.Query().Include(c => c.DisplayLst).FirstAsync(c => c.TechnicalId == action.CredentialTemplateId);
            var display = new CredentialTemplateDisplay
            {
                BackgroundColor = action.BackgroundColor,
                Description = action.Description,
                Id = Guid.NewGuid().ToString(),
                Locale = action.Locale,
                LogoUrl = action.LogoUrl,
                LogoAltText = action.LogoAltText,
                Name = action.Name,
                TextColor = action.TextColor
            };
            credentialTemplate.DisplayLst.Add(display);
            await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddCredentialTemplateDisplaySuccessAction { Display = display });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedCredentialSubjectsAction action, IDispatcher dispatcher)
        {
            var credentialTemplate = await _credentialTemplateRepository.Query().Include(c => c.Parameters).FirstAsync(c => c.TechnicalId == action.TechnicalId);
            credentialTemplate.Parameters = credentialTemplate.Parameters.Where(p => !action.ParameterIds.Contains(p.Id)).ToList();
            await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedCredentialSubjectsSuccessAction { ParameterIds = action.ParameterIds, TechnicalId = action.TechnicalId });
        }

        [EffectMethod]
        public async Task Handle(UpdateW3CCredentialTemplateTypesAction action, IDispatcher dispatcher)
        {
            var credentialTemplate = await _credentialTemplateRepository.Query().Include(c => c.Parameters).FirstAsync(c => c.TechnicalId == action.TechnicalId);
            var w3cCredentialTemplate = new W3CCredentialTemplate(credentialTemplate);
            w3cCredentialTemplate.ReplaceTypes(action.ConcatenatedTypes.Split(';'));
            credentialTemplate.Parameters = w3cCredentialTemplate.Parameters;
            await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateW3CCredentialTemplateTypesSuccessAction { ConcatenatedTypes = action.ConcatenatedTypes, TechnicalId = action.TechnicalId });
        }

        [EffectMethod]
        public async Task Handle(AddW3CCredentialTemplateCredentialSubjectAction action, IDispatcher dispatcher)
        {
            var credentialTemplate = await _credentialTemplateRepository.Query().Include(c => c.Parameters).FirstAsync(c => c.TechnicalId == action.TechnicalId);
            var w3cCredentialTemplate = new W3CCredentialTemplate(credentialTemplate);
            var parameter = w3cCredentialTemplate.AddCredentialSubject(action.ClaimName, action.Subject);
            credentialTemplate.Parameters = w3cCredentialTemplate.Parameters;
            await _credentialTemplateRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddW3CCredentialTemplateCredentialSubjectSuccessAction { TechnicalId = action.TechnicalId, ClaimName = action.ClaimName, Subject = action.Subject, ParameterId = parameter.Id });
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

    public class GetCredentialTemplateAction
    {
        public string Id { get; set; }
    }

    public class GetCredentialTemplateSuccessAction
    {
        public CredentialTemplate CredentialTemplate { get; set; }
    }

    public class ToggleAllCredentialTemplateDisplayAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleCredentialTemplateDisplayAction
    {
        public string Id { get; set; }
        public string DisplayId { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedCredentialTemplateDisplayAction
    {
        public string Id { get; set; }
        public IEnumerable<string> DisplayIds { get; set; } 
    }

    public class RemoveSelectedCredentialTemplateDisplaySuccessAction
    {
        public string Id { get; set; }
        public IEnumerable<string> DisplayIds { get; set; }
    }

    public class AddCredentialTemplateDisplayAction
    {
        public string CredentialTemplateId { get; set; }
        public string Name { get; set; } = null!;
        public string Locale { get; set; } = null!;
        public string? Description { get; set; } = null;
        public string? LogoUrl { get; set; } = null;
        public string? LogoAltText { get; set; } = null;
        public string? BackgroundColor { get; set; } = null;
        public string? TextColor { get; set; } = null;
    }

    public class AddCredentialTemplateDisplaySuccessAction
    {
        public CredentialTemplateDisplay Display { get; set; }
    }

    public class ToggleAllCredentialSubjectsAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleCredentialSubjectAction
    {
        public string ClaimName { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedCredentialSubjectsAction
    {
        public string TechnicalId { get; set; }
        public IEnumerable<string> ParameterIds { get; set; }
    }

    public class RemoveSelectedCredentialSubjectsSuccessAction
    {
        public string TechnicalId { get; set; }
        public IEnumerable<string> ParameterIds { get; set; }
    }

    public class UpdateW3CCredentialTemplateTypesAction
    {
        public string TechnicalId { get; set; }
        public string ConcatenatedTypes { get; set; }
    }

    public class UpdateW3CCredentialTemplateTypesSuccessAction
    {
        public string TechnicalId { get; set; }
        public string ConcatenatedTypes { get; set; }
    }

    public class AddW3CCredentialTemplateCredentialSubjectAction
    {
        public string TechnicalId { get; set; }
        public string ClaimName { get; set; }
        public W3CCredentialSubject Subject { get; set; }
    }

    public class AddW3CCredentialTemplateCredentialSubjectSuccessAction
    {
        public string TechnicalId { get; set; }
        public string ParameterId { get; set; }
        public string ClaimName { get; set; }
        public W3CCredentialSubject Subject { get; set; }
    }
}
