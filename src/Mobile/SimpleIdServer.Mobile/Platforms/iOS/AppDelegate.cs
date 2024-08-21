using Foundation;
using SimpleIdServer.Mobile.Services;

namespace SimpleIdServer.Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool OpenUrl(UIKit.UIApplication application, NSUrl url, NSDictionary options)
    {
        var urlStr = url.AbsoluteString;
        if (string.IsNullOrWhiteSpace(urlStr) || !urlStr.StartsWith("openid-credential-offer")) return true;
        Uri? uri = null;
        if (!Uri.TryCreate(urlStr, UriKind.Absolute, out uri)) return true;
        var queries = uri.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);
        CredentialOfferListener.New().Receive(queries);
        return true;
    }
}
