using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SimpleIdServer.Mobile.Clients;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using SimpleIdServer.Mobile.ViewModels;
using SimpleIdServer.WalletClient.Stores;
using ZXing.Net.Maui.Controls;

#if ANDROID
using SimpleIdServer.Mobile.Handlers;
#endif

namespace SimpleIdServer.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureLifecycleEvents(e =>
			{
#if ANDROID
				e.AddAndroid(ad =>
				{
					ad.OnStop(a =>
					{

					});
				});
#endif
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				handlers.AddHandler<CollectionView, SidCollectionViewHandler>();
#endif
			})
            .UseBarcodeReader()
            .UseMauiCommunityToolkit()
			.RegisterFirebaseServices()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddWalletClient();
		builder.Services.AddTransient<ISidServerClient, SidServerClient>();
		builder.Services.AddTransient<IPromptService, PromptService>();
        builder.Services.AddTransient<IOTPService, OTPService>();
		builder.Services.AddTransient<INavigationService, NavigationService>();
		builder.Services.AddTransient<IUrlService, UrlService>();
		builder.Services.AddTransient<Factories.IHttpClientFactory, Factories.HttpClientFactory>();
		builder.Services.AddSingleton(new OtpListState());
		builder.Services.AddSingleton(new CredentialListState());
		builder.Services.AddSingleton(new VerifiableCredentialListState());
		builder.Services.AddSingleton(new MobileSettingsState());
		builder.Services.AddSingleton(new DidRecordState());
        builder.Services.AddTransient<EnrollPage>();
		builder.Services.AddTransient<NotificationPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<ViewOtpListPage>();
		builder.Services.AddTransient<QRCodeScannerPage>();
		builder.Services.AddTransient<WalletPage>();
		builder.Services.AddTransient<PinModal>();
		builder.Services.AddTransient<ViewCredentialOffer>();
		builder.Services.AddTransient<ViewCredentialListPage>();
		builder.Services.AddTransient<QRCodeScannerViewModel>();
        builder.Services.AddTransient<EnrollViewModel>();
		builder.Services.AddTransient<SettingsPageViewModel>();
		builder.Services.AddTransient<ViewOtpListViewModel>();
		builder.Services.AddTransient<ViewCredentialListViewModel>();
		builder.Services.AddTransient<NotificationViewModel>();
		builder.Services.AddTransient<WalletViewModel>();
		builder.Services.AddTransient<PinModalViewModel>();
		builder.Services.AddTransient<ViewCredentialOfferViewModel>();
		builder.Services.RemoveAll<IVcStore>();
		builder.Services.AddTransient<IVcStore, MobileVcStore>();
		builder.Services.Configure<MobileOptions>(o =>
		{
			o.WsServer = "wss://gotify.simpleidserver.com";
        });
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
