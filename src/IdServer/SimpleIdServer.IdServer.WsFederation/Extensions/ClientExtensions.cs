using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.WsFederation.Extensions
{
    public static class ClientExtensions
    {
        private static string WSFEDERATION_ENABLED_NAME = "WsFederationEnabled";

        public static bool IsWsFederationEnabled(this Client client)
        {
            bool result;
            if (!client.Parameters.ContainsKey(WSFEDERATION_ENABLED_NAME) || !bool.TryParse(client.Parameters.First(p => p.Key == WSFEDERATION_ENABLED_NAME).Value, out result))
                return false;

            return result;
        }

        public static void SetWsFederationEnabled(this Client client, bool isEnabled)
        {
            var parameters = client.Parameters;
            if (!parameters.ContainsKey(WSFEDERATION_ENABLED_NAME))
                parameters.Add(WSFEDERATION_ENABLED_NAME, isEnabled.ToString());
            else
                parameters[WSFEDERATION_ENABLED_NAME] = isEnabled.ToString();
            client.Parameters = parameters;
        }
    }
}
