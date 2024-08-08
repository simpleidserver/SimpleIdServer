using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.CloudMessaging;
#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
using SimpleIdServer.Mobile.Services;
#else
using Plugin.Firebase.Core.Platforms.Android;
using Plugin.Firebase.Crashlytics;
using SimpleIdServer.Mobile;
using SimpleIdServer.Mobile.Services;
using System.Runtime.CompilerServices;
#endif

namespace Microsoft.Maui.Hosting;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(evts =>
        {
#if IOS
            evts.AddiOS(iOS => iOS.FinishedLaunching((_,__) => {
                CrossFirebase.Initialize();
                return false;
            }));
#else
            evts.AddAndroid(android => android.OnCreate((activity, _) =>
            {
                CrossFirebase.Initialize(activity);
                var action = activity.Intent?.Action;
                var data = activity.Intent?.Data?.ToString();
                if(action == Android.Content.Intent.ActionView && data is not null)
                {
                    Task.Run(() => HandleIntentData(data));
                }
            }));
#endif
        });

        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        return builder;
    }

    private static void HandleIntentData(string data)
    {
        if (string.IsNullOrWhiteSpace(data) || !data.StartsWith("openid-credential-offer")) return;
        Uri? uri = null;
        if (!Uri.TryCreate(data, UriKind.Absolute, out uri)) return;
        var queries = uri.Query.TrimStart('?').Split('&').Select(t => t.Split('=')).ToDictionary(r => r[0], r => r[1]);
        CredentialOfferListener.New().Receive(queries);
    }
}
