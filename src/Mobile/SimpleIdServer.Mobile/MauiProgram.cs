using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
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
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddTransient<ICertificateStore, CertificateStore>();
		builder.Services.AddTransient<IPromptService, PromptService>();
		builder.Services.AddTransient<EnrollPage>();
		builder.Services.AddTransient<QRCodeScannerPage>();
		builder.Services.AddTransient<QRCodeScannerViewModel>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
