using Microsoft.Extensions.Options;
using SimpleIdServer.Mobile.Services;
using System.Windows.Input;
using ZXing.Net.Maui;
namespace SimpleIdServer.Mobile.ViewModels;

public class QRCodeScannerViewModel : BaseQRCodeViewModel
{
    public QRCodeScannerViewModel(IPromptService promptService, IOptions<MobileOptions> options) : base(promptService, options)
    {
        ScanQRCodeCommand = new Command<BarcodeDetectionEventArgs>(async (c) =>
        {
            if (c == null || c.Results == null || !c.Results.Any()) return;
            var firstResult = c.Results.First().Value;
            await ScanQRCode(firstResult);
        });
    }

    public ICommand ScanQRCodeCommand { get; private set; }
}
