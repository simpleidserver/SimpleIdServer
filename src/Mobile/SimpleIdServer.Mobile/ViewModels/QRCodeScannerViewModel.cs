using Microsoft.Extensions.Options;
using Plugin.Firebase.CloudMessaging;
using SimpleIdServer.IdServer.U2FClient;
using SimpleIdServer.Mobile.DTOs;
using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
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
        private readonly IPromptService _promptService;
        private readonly MobileOptions _options;

        public QRCodeScannerViewModel(IPromptService promptService, IOptions<MobileOptions> options)
        {
            _promptService = promptService;
            _options = options.Value;
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
                var qrCodeResult = firstResult.Value;
                if (string.IsNullOrWhiteSpace(qrCodeResult)) return;
                var beginResult = await ReadQRCode(qrCodeResult);
                var attestationBuilder = new FIDOU2FAttestationBuilder();
                var enrollResponse = attestationBuilder.BuildEnrollResponse(new EnrollParameter
                {
                    Challenge = beginResult.CredentialCreateOptions.Challenge,
                    Rp = beginResult.CredentialCreateOptions.Rp.Id
                });
                await EndRegister(beginResult, enrollResponse);
                var credentialRecord = new CredentialRecord(enrollResponse.CredentialId, enrollResponse.AttestationCertificate.AttestationCertificate, enrollResponse.AttestationCertificate.PrivateKey);
                await App.Database.AddCredentialRecord(credentialRecord);
                IsLoading = false;
                await _promptService.ShowAlert("Success", "Your mobile device has been enrolled");
                await Shell.Current.GoToAsync("..");
            }
            catch
            {
                IsLoading = false;
                await _promptService.ShowAlert("Error", "An error occured while trying to parse the QR Code");
            }
            finally
            {
                IsLoading = false;
            }

            async Task<BeginU2FRegisterResult> ReadQRCode(string qrCodeValue)
            {
                var handler = new HttpClientHandler();
                if (_options.IgnoreHttps) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    return true;
                };
                var qrCodeResult = JsonSerializer.Deserialize<QRCodeResult>(qrCodeValue);
                using (var httpClient = new HttpClient(handler))
                {
                    var requestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(SanitizeEndRegisterUrl(qrCodeResult.ReadQRCodeURL))
                    };
                    var httpResponse = await httpClient.SendAsync(requestMessage);
                    httpResponse.EnsureSuccessStatusCode();
                    var json = await httpResponse.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<BeginU2FRegisterResult>(json);
                }
            }

            async Task EndRegister(BeginU2FRegisterResult beginResult, EnrollResult enrollResponse)
            {
                var fcmToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                var handler = new HttpClientHandler();
                if (_options.IgnoreHttps) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    return true;
                };
                using (var httpClient = new HttpClient(handler))
                {
                    var deviceInfo = DeviceInfo.Current;
                    var deviceData = new Dictionary<string, string>
                    {
                        { "device_type", deviceInfo.Platform.ToString() },
                        { "model", deviceInfo.Model },
                        { "manufacturer", deviceInfo.Manufacturer },
                        { "name", deviceInfo.Name },
                        { "version", deviceInfo.VersionString },
                        { "push_token", fcmToken },
                        { "push_type", _options.PushType }
                    };
                    var dic = new Dictionary<string, object>
                    {
                        { "login", beginResult.Login },
                        { "session_id", beginResult.SessionId },
                        { "attestation", enrollResponse.Response },
                        { "device_data", deviceData }
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
            }

            string SanitizeEndRegisterUrl(string url) => _options.IsDev ? url.Replace("localhost", "192.168.50.125") : url;
        }
    }
}
