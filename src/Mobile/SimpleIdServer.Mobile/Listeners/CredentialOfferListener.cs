namespace SimpleIdServer.Mobile.Services;

public class CredentialOfferListener
{
    private static CredentialOfferListener _instance;

    private CredentialOfferListener()
    {
        
    }

    public event EventHandler<CredentialOfferEventArgs> CredentialOfferReceived;

    public static CredentialOfferListener New()
    {
        if(_instance == null)
        {
            _instance = new CredentialOfferListener();
        }

        return _instance;
    }

    public void Receive(Dictionary<string, string> parameters, Func<Task> callback = null)
    {
        if (CredentialOfferReceived != null) CredentialOfferReceived(this, new CredentialOfferEventArgs(parameters, callback));
    }
}

public class CredentialOfferEventArgs : EventArgs
{
    public CredentialOfferEventArgs(Dictionary<string, string> parameters, Func<Task> callback = null)
    {
        Parameters = parameters;
        Callback = callback;
    }

    public Dictionary<string, string> Parameters { get; private set; }
    public Func<Task> Callback { get; private set; }
    public Func<Task> ErrorCallback { get; private set; }
}
