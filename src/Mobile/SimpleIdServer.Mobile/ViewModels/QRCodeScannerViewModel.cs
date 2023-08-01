using SimpleIdServer.IdServer.U2FClient;
using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using ZXing.Net.Maui;

namespace SimpleIdServer.Mobile.ViewModels
{
    public class QRCodeScannerViewModel : INotifyPropertyChanged
    {
        private bool _isNotScanned = true;
        private bool _isLoading = false;
        private readonly ICertificateStore _certificateStore;
        private readonly IPromptService _promptService;

        public QRCodeScannerViewModel(ICertificateStore certificateStore, IPromptService promptService)
        {
            _certificateStore = certificateStore;
            _promptService = promptService;
            ScanQRCodeCommand = new Command<BarcodeDetectionEventArgs>(async (c) =>
            {
                await ScanQRCode(c);
            });
            CloseCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ScanQRCodeCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public bool IsNotScanned
        {
            get => _isNotScanned;
            set
            {
                if(_isNotScanned != value)
                {
                    _isNotScanned = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = true;
                    OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private async Task ScanQRCode(BarcodeDetectionEventArgs e)
        {
            if (e == null || e.Results == null || !e.Results.Any() || IsLoading) return;
            IsLoading = true;
            try
            {
                var firstResult = e.Results.First();
                var value = firstResult.Value;
                if (string.IsNullOrWhiteSpace(value)) return;
                var beginResult = JsonSerializer.Deserialize<BeginU2FRegisterResult>(value);
                if (beginResult == null) return;

                var attestationBuilder = new FIDOU2FAttestationBuilder();
                var enrollResponse = attestationBuilder.BuildEnrollResponse(new EnrollParameter
                {
                    Challenge = beginResult.CredentialCreateOptions.Challenge,
                    Rp = beginResult.CredentialCreateOptions.Rp.Id
                });
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    return true;
                };
                using (var httpClient = new HttpClient(handler))
                {
                    var dic = new Dictionary<string, object>
                    {
                        { "login", beginResult.Login },
                        { "session_id", beginResult.SessionId },
                        { "attestation", enrollResponse.Response }
                    };
                    var requestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(SanitizeEndRegisterUrl(beginResult.EndRegisterUrl)),
                        Content = new StringContent(JsonSerializer.Serialize(dic), Encoding.UTF8, "application/json")
                    };
                    var httpResponse = await httpClient.SendAsync(requestMessage);
                    httpResponse.EnsureSuccessStatusCode();
                }
                await _certificateStore.Add(enrollResponse.CredentialId, new CertificateRecord(enrollResponse.AttestationCertificate.AttestationCertificate, enrollResponse.AttestationCertificate.PrivateKey));
                IsLoading = false;
                await _promptService.ShowAlert("Success", "Your mobile device has been enrolled");
                await Shell.Current.GoToAsync("..");
            }
            catch
            {
                await _promptService.ShowAlert("Error", "An error occured while trying to parse the QR Code");
            }
            finally
            {
                IsLoading = false;
            }

            string SanitizeEndRegisterUrl(string url) => url.Replace("localhost", "10.0.2.2");
        }
    }
}
