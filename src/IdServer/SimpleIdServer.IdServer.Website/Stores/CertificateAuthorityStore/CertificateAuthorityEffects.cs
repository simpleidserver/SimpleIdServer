// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Website.Stores.CertificateAuthorityStore
{
    public class CertificateAuthorityEffects
    {
        private readonly IDbContextFactory<StoreDbContext> _factory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public CertificateAuthorityEffects(IDbContextFactory<StoreDbContext> factory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage)
        {
            _factory = factory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchCertificateAuthoritiesAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using(var dbContext = _factory.CreateDbContext())
            {
                IQueryable<CertificateAuthority> query = dbContext.CertificateAuthorities.Include(c => c.Realms).Where(c => c.Realms.Any(r => r.Name == realm)).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(action.Filter))
                    query = query.Where(SanitizeExpression(action.Filter));

                if (!string.IsNullOrWhiteSpace(action.OrderBy))
                    query = query.OrderBy(SanitizeExpression(action.OrderBy));

                var nb = query.Count();
                var clients = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
                dispatcher.Dispatch(new SearchCertificateAuthoritiesSuccessAction { CertificateAuthorities = clients, Count = nb });
            }

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
        public Task Handle(ImportCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            var store = new X509Store(action.StoreName, action.StoreLocation);
            try
            {
                store.Open(OpenFlags.ReadOnly);
            }
            catch
            {
                dispatcher.Dispatch(new GenerateCertificateAuthorityFailureAction { ErrorMessage = Resources.Global.CannotReadCertificateStore });
                return Task.CompletedTask;
            }

            var certificate = store.Certificates.Find(action.FindType, action.FindValue, true).FirstOrDefault();
            if(certificate == null)
            {
                dispatcher.Dispatch(new GenerateCertificateAuthorityFailureAction { ErrorMessage = Resources.Global.CertificateDoesntExist });
                return Task.CompletedTask;
            }

            try
            {
                if (!certificate.HasPrivateKey || certificate.PrivateKey == null)
                {
                    dispatcher.Dispatch(new GenerateCertificateAuthorityFailureAction { ErrorMessage = Resources.Global.CertificateDoesntHavePrivateKey });
                    return Task.CompletedTask;
                }
            }
            catch
            {
                dispatcher.Dispatch(new GenerateCertificateAuthorityFailureAction { ErrorMessage = Resources.Global.CertificateDoesntHavePrivateKey });
                return Task.CompletedTask;
            }


            var certificateAuthority = CertificateAuthorityBuilder.Import(certificate, action.StoreLocation, action.StoreName, action.FindType, action.FindValue).Build();
            dispatcher.Dispatch(new GenerateCertificateAuthoritySuccessAction { CertificateAuthority = certificateAuthority });
            return Task.CompletedTask;
        }

        [EffectMethod]
        public async Task Handle(SaveCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                action.CertificateAuthority.Realms.Add(activeRealm);
                dbContext.CertificateAuthorities.Add(action.CertificateAuthority);
                await dbContext.SaveChangesAsync();
            }

            dispatcher.Dispatch(new SaveCertificateAuthoritySuccessAction { CertificateAuthority = action.CertificateAuthority });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedCertificateAuthoritiesAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var cas = await dbContext.CertificateAuthorities.Where(c => action.Ids.Contains(c.Id)).ToListAsync();
                dbContext.CertificateAuthorities.RemoveRange(cas);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new RemoveSelectedCertificateAuthoritiesSuccessAction { Ids = action.Ids });
            }    
        }

        [EffectMethod]
        public async Task Handle(GetCertificateAuthorityAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var ca = await dbContext.CertificateAuthorities.Include(c => c.ClientCertificates).FirstOrDefaultAsync(a => a.Id == action.Id);
                var store = new IdServer.Stores.CertificateAuthorityStore(null);
                var certificate = store.Get(ca);
                dispatcher.Dispatch(new GetCertificateAuthoritySuccessAction { CertificateAuthority = ca, Certificate = certificate });
            }
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedClientCertificatesAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var ca = await dbContext.CertificateAuthorities.Include(c => c.ClientCertificates).FirstAsync(c => c.Id == action.CertificateAuthorityId);
                ca.ClientCertificates = ca.ClientCertificates.Where(c => !action.CertificateClientIds.Contains(c.Id)).ToList();
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new RemoveSelectedClientCertificatesSuccessAction { CertificateAuthorityId = action.CertificateAuthorityId, CertificateClientIds = action.CertificateClientIds });
            }
        }

        [EffectMethod]
        public async Task Handle(AddClientCertificateAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var ca = await dbContext.CertificateAuthorities.Include(c => c.ClientCertificates).FirstAsync(c => c.Id == action.CertificateAuthorityId);
                var store = new IdServer.Stores.CertificateAuthorityStore(null);
                var certificate = store.Get(ca);
                var pem = KeyGenerator.GenerateClientCertificate(certificate, action.SubjectName, action.NbDays);
                var record = new ClientCertificate
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = action.SubjectName,
                    PublicKey = pem.PublicKey,
                    PrivateKey = pem.PrivateKey,
                    StartDateTime = DateTime.UtcNow,
                    EndDateTime = DateTime.UtcNow.AddDays(action.NbDays)
                };
                ca.ClientCertificates.Add(record);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddClientCertificateSuccessAction { CertificateAuthorityId = action.CertificateAuthorityId, ClientCertificate = record });
            }
        }

        private async Task<string> GetRealm()
        {
            if (!_options.IsReamEnabled) return SimpleIdServer.IdServer.Constants.DefaultRealm;
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

    public class RemoveSelectedCertificateAuthoritiesSuccessAction
    {
        public IEnumerable<string> Ids { get; set; }
    }

    public class GenerateCertificateAuthorityAction
    {
        public string SubjectName { get; set; }
        public int NumberOfDays { get; set; }
    }

    public class ImportCertificateAuthorityAction
    {
        public StoreLocation StoreLocation { get; set; }
        public StoreName StoreName { get; set; }
        public X509FindType FindType { get; set; }
        public string FindValue { get; set; }
    }

    public class GenerateCertificateAuthoritySuccessAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }

    public class GenerateCertificateAuthorityFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class SaveCertificateAuthorityAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }

    public class SaveCertificateAuthoritySuccessAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
    }

    public class SelectCertificateAuthoritySourceAction
    {
        public CertificateAuthoritySources Source { get; set; }
    }

    public class GetCertificateAuthorityAction
    {
        public string Id { get; set; }
    }

    public class GetCertificateAuthoritySuccessAction
    {
        public CertificateAuthority CertificateAuthority { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }

    public class ToggleClientCertificateSelectionAction
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ToggleAllClientCertificatesSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class RemoveSelectedClientCertificatesAction
    {
        public string CertificateAuthorityId { get; set; }
        public IEnumerable<string> CertificateClientIds { get; set; }
    }

    public class RemoveSelectedClientCertificatesSuccessAction
    {
        public string CertificateAuthorityId { get; set; }
        public IEnumerable<string> CertificateClientIds { get; set; }
    }

    public class AddClientCertificateAction
    {
        public string CertificateAuthorityId { get; set; }
        public string SubjectName { get; set; }
        public int NbDays { get; set; }
    }

    public class AddClientCertificateSuccessAction
    {
        public string CertificateAuthorityId { get; set; }
        public ClientCertificate ClientCertificate { get; set; }
    }
}
