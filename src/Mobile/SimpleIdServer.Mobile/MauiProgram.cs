using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Firebase.CloudMessaging;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.ViewModels;
using ZXing.Net.Maui.Controls;

namespace SimpleIdServer.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseBarcodeReader()
            .UseMauiCommunityToolkit()
			.RegisterFirebaseServices()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddTransient<IPromptService, PromptService>();
		builder.Services.AddTransient<EnrollPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<QRCodeScannerPage>();
		builder.Services.AddTransient<QRCodeInputPage>();
		builder.Services.AddTransient<QRCodeScannerViewModel>();
        builder.Services.AddTransient<EnrollViewModel>();
		builder.Services.AddTransient<SettingsPageViewModel>();
        builder.Services.Configure<MobileOptions>(o =>
		{
			o.PushType = "firebase";
			o.IsDev = true;
        });
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
