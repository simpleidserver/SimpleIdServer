// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.CertificateAuthorityStore
{
    public class CertificateAuthorityEffects
    {
        private readonly ICertificateAuthorityRepository _certificateAuthorityRepository;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly DbContextOptions<StoreDbContext> _options;

        public CertificateAuthorityEffects(ICertificateAuthorityRepository certificateAuthorityRepository, ProtectedSessionStorage sessionStorage, DbContextOptions<StoreDbContext> options)
        {
            _certificateAuthorityRepository = certificateAuthorityRepository;
            _sessionStorage = sessionStorage;
            _options = options;
        }

        [EffectMethod]
        public async Task Handle(SearchCertificateAuthoritiesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            IQueryable<CertificateAuthority> query = _certificateAuthorityRepository.Query().Include(c => c.Realms).Where(c => c.Realms.Any(r => r.Name == realm)).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var nb = query.Count();
            var clients = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchCertificateAuthoritiesSuccessAction { CertificateAuthorities = clients, Count = nb });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public Task Handle(GenerateCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            var certificateAuthority = CertificateAuthorityBuilder.Create(action.SubjectName, numberOfDays: action.NumberOfDays).Build();
            dispatcher.Dispatch(new GenerateCertificateAuthoritySuccessAction { CertificateAuthority = certificateAuthority });
            return Task.CompletedTask;
        }

        [EffectMethod]
        public async Task Handle(SaveCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            using (var dbContext = new StoreDbContext(_options))
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                action.CertificateAuthority.Realms.Add(activeRealm);
                dbContext.CertificateAuthorities.Add(action.CertificateAuthority);
                await dbContext.SaveChangesAsync();
            }

            dispatcher.Dispatch(new SaveCertificateAuthoritySuccessAction { CertificateAuthority = action.CertificateAuthority });
        }

        private async Task<string> GetRealm()
        {
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchCertificateAuthoritiesAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchCertificateAuthoritiesSuccessAction
    {
        public IEnumerable<CertificateAuthority> CertificateAuthorities { get; set; } = new List<CertificateAuthority>();
        public int Count { get; set; }
    }

    public class ToggleAllCertificateAuthoritySelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleCertificateAuthoritySelectionAction
    {
        public bool IsSelected { get; set; }
        public string Id { get; set; }
    }

    public class RemoveSelectedCertificateAuthoritiesAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class GenerateCertificateAuthorityAction
    {
        public string SubjectName { get; set; }
        public int NumberOfDays { get; set; }
    }

    public class GenerateCertificateAuthoritySuccessAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }

    public class SaveCertificateAuthorityAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }

    public class SaveCertificateAuthoritySuccessAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }
}
