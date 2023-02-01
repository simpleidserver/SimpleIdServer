using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    public static class ClientReducers
    {
        [ReducerMethod]
        public static ClientState ReduceSearchClientsAction(ClientState state, SearchClientsAction act) => new(isLoading: true, clients: new List<Client>());

        [ReducerMethod]
        public static ClientState ReduceSearchClientsActionResult(ClientState state, SearchClientsActionResult act)
        {
            return state with
            {
                IsLoading = false,
                Clients = act.Clients,
                Count = act.Clients.Count()
            };
        }
    }
}
