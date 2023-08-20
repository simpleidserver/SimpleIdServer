using Microsoft.Extensions.Options;
using SimpleIdServer.Mobile.Services;
using System.Windows.Input;

namespace SimpleIdServer.Mobile.ViewModels
{
    public class QRCodeInputViewModel : BaseQRCodeViewModel
    {
        private string _qrCode;

        public QRCodeInputViewModel(IPromptService promptService, IOptions<MobileOptions> options) : base(promptService, options)
        {
            SubmitQRCodeCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(QRCode)) return;
                await ScanQRCode(QRCode);
            });
        }

        public ICommand SubmitQRCodeCommand { get; private set; }

        public string QRCode
        {
            get => _qrCode;
            set
            {
                if (_qrCode != value)
                {
                    _qrCode = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
