// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    public class ClientEffects
    {
        private readonly IClientRepository _clientRepository;
        private readonly IScopeRepository _scopeRepository;

        public ClientEffects(IClientRepository clientRepository, IScopeRepository scopeRepository)
        {
            _clientRepository = clientRepository;
            _scopeRepository = scopeRepository;
        }

        [EffectMethod]
        public async Task Handle(SearchClientsAction action, IDispatcher dispatcher)
        {
            IQueryable<Client> query = _clientRepository.Query().Include(c => c.Translations).Include(c => c.Scopes).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var clients = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchClientsSuccessAction { Clients = clients });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(AddSpaClientAction action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, action.RedirectionUrls, dispatcher)) return;
            var scopes = await _scopeRepository.Query().Where(s => s.Name == Constants.StandardScopes.OpenIdScope.Name || s.Name == Constants.StandardScopes.Profile.Name).ToListAsync(CancellationToken.None);
            var newClientBuilder = ClientBuilder.BuildUserAgentClient(action.ClientId, Guid.NewGuid().ToString(), action.RedirectionUrls.ToArray())
                .AddScope(scopes.ToArray());
            if (!string.IsNullOrWhiteSpace(action.ClientName))
                newClientBuilder.SetClientName(action.ClientName);
            var newClient = newClientBuilder.Build();
            _clientRepository.Add(newClient);
            await _clientRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.SPA });
        }

        [EffectMethod]
        public async Task Handle(AddMachineClientApplication action, IDispatcher dispatcher)
        {
            if (!await ValidateAddClient(action.ClientId, new List<string>(), dispatcher)) return;
            var newClientBuilder = ClientBuilder.BuildApiClient(action.ClientId, action.ClientSecret);
            if (!string.IsNullOrWhiteSpace(action.ClientName))
                newClientBuilder.SetClientName(action.ClientName);
            var newClient = newClientBuilder.Build();
            _clientRepository.Add(newClient);
            await _clientRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new AddClientSuccessAction { ClientId = action.ClientId, ClientName = action.ClientName, Language = newClient.Translations.FirstOrDefault()?.Language, ClientType = ClientTypes.MACHINE });
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedClientsAction action, IDispatcher dispatcher)
        {
            var clients = await _clientRepository.Query().Where(c => action.ClientIds.Contains(c.ClientId)).ToListAsync(CancellationToken.None);
            _clientRepository.DeleteRange(clients);
            await _clientRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedClientsSuccessAction { ClientIds = action.ClientIds });
        }

        private async Task<bool> ValidateAddClient(string clientId, IEnumerable<string> redirectionUrls, IDispatcher dispatcher)
        {
            var errors = new List<string>();
            foreach (var redirectionUrl in redirectionUrls)
                if (!ValidateRedirectionUrl(redirectionUrl, out string errorMessage))
                    errors.Add(errorMessage);

            if(errors.Any())
            {
                dispatcher.Dispatch(new AddClientFailureAction { ClientId = clientId, ErrorMessage = string.Join(",", errors) });
                return false;
            }

            var existingClient = await _clientRepository.Query().AsNoTracking().AnyAsync(c => c.ClientId == clientId, CancellationToken.None);
            if (existingClient)
            {
                dispatcher.Dispatch(new AddClientFailureAction { ClientId = clientId, ErrorMessage = Global.ClientAlreadyExists });
                return false;
            }

            return true;
        }

        private bool ValidateRedirectionUrl(string redirectionUrl, out string errorMessage)
        {
            errorMessage = null;
            if (string.IsNullOrWhiteSpace(redirectionUrl) || !Uri.IsWellFormedUriString(redirectionUrl, UriKind.Absolute))
            {
                errorMessage = string.Format(Global.InvalidRedirectUrl, redirectionUrl);
                return false;
            }

            Uri.TryCreate(redirectionUrl, UriKind.Absolute, out Uri uri);
            if (!string.IsNullOrWhiteSpace(uri.Fragment))
                errorMessage = string.Format(Global.RedirectUriContainsFragment, redirectionUrl);

            return errorMessage == null;
        }
    }

    public class SearchClientsAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchClientsSuccessAction
    {
        public IEnumerable<Client> Clients { get; set; } = new List<Client>();
    }

    public class AddSpaClientAction
    {
        public IEnumerable<string> RedirectionUrls { get; set; } = new List<string>();
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
    }

    public class AddMachineClientApplication
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string? ClientName { get; set; } = null;
    }

    public class AddClientFailureAction
    {
        public string ClientId { get; set; } = null!;
        public string ErrorMessage { get; set; } = null;
    }

    public class AddClientSuccessAction
    {
        public string ClientId { get; set; } = null!;
        public string? ClientName { get; set; } = null;
        public string? Language { get; set; } = null;
        public ClientTypes ClientType { get; set; }
    }

    public class RemoveSelectedClientsAction 
    {
        public IEnumerable<string> ClientIds { get; set; } = new List<string>();
    }

    public class RemoveSelectedClientsSuccessAction
    {
        public IEnumerable<string> ClientIds { get; set; } = new List<string>();
    }

    public class ToggleClientSelectionAction
    {
        public bool IsSelected { get; set; } = false;
        public string ClientId { get; set; } = null!;
    }

    public class ToggleAllClientSelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }
}
