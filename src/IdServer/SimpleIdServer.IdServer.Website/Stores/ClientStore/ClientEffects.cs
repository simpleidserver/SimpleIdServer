using Fluxor;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    public class ClientEffects
    {
        private readonly IClientRepository _clientRepository;

        public ClientEffects(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [EffectMethod]
        public async Task Handle(SearchClientsAction action, IDispatcher dispatcher)
        {
            var clients = await _clientRepository.Query().Include(c => c.Translations).Include(c => c.Scopes).AsNoTracking().ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchClientsActionResult { Clients = clients });
        }
    }

    public class SearchClientsAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchClientsActionResult
    {
        public IEnumerable<Client> Clients { get; set; } = new List<Client>();
    }
}
