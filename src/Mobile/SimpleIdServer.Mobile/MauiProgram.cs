using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Mobile.Helpers;
#if ANDROID
using SimpleIdServer.Mobile.Platforms.Android.Services;
#endif
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
		builder.Services.AddTransient<QRCodeInputViewModel>();
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

		AddPlatformSpecificItems(builder);

        return builder.Build();
	}

	private static void AddPlatformSpecificItems(MauiAppBuilder builder)
	{
#if ANDROID
        builder.Services.AddSingleton<IPlatformHelpers, DroidPlatformHelpers>();
		builder.Services.AddSingleton<IBluetoothService, AndroidBluetoothService>();
#else
        builder.Services.AddSingleton<IPlatformHelpers, iOSPlatformHelpers>();
#endif
    }
}
