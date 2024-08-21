using Foundation;
using SimpleIdServer.Mobile.Services;

namespace SimpleIdServer.Mobile;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp(); 
    
    [Export("application:continueUserActivity:restorationHandler:")]
    public override bool ContinueUserActivity(UIKit.UIApplication application, NSUserActivity userActivity, UIKit.UIApplicationRestorationHandler completionHandler)
    {
        if (userActivity != null)
        {
            var url = userActivity.WebPageUrl?.ToString();
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("openid-credential-offer")) return true;
            Uri? uri = null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri)) return true;
            var queries = uri.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);
            CredentialOfferListener.New().Receive(queries);
        }

        return true;
    }
}
