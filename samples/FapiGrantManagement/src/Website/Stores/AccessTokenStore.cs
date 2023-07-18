namespace Website.Stores;

public class AccessTokenStore
{
    private static AccessTokenStore _instance;
    private Dictionary<string, string> _accessTokens = new Dictionary<string, string>();

    private AccessTokenStore() { }

    public string GetAccessToken(string bankName) => _accessTokens[bankName];

    public void Add(string bankName, string token)
    {
        if(_accessTokens.ContainsKey(bankName)) _accessTokens.Remove(bankName);
        _accessTokens.Add(bankName, token);
    }

    public static AccessTokenStore Instance()
    {
        if(_instance == null) _instance = new AccessTokenStore();
        return _instance;
    }
}