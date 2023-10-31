using Fido2NetLib;
using Microsoft.Extensions.Options;
using Plugin.Firebase.CloudMessaging;
using SimpleIdServer.IdServer.U2FClient;
using SimpleIdServer.Mobile.DTOs;
using SimpleIdServer.Mobile.Models;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using ZXing.Net.Maui;
#if IOS
using Firebase.CloudMessaging;
#endif

namespace SimpleIdServer.Mobile.ViewModels;

public class QRCodeScannerViewModel
{
    private bool _isLoading = false;
    private readonly IPromptService _promptService;
    private readonly IOTPService _otpService;
    private readonly INavigationService _navigationService;
    private readonly OtpListState _otpListState;
    private readonly CredentialListState _credentialListState;
    private readonly MobileOptions _options;
    private SemaphoreSlim _lck = new SemaphoreSlim(1, 1);

    public QRCodeScannerViewModel(IPromptService promptService, IOTPService otpService, INavigationService navigationService, OtpListState otpListState, CredentialListState credentialListState, IOptions<MobileOptions> options)
    {
        _promptService = promptService;
        _otpService = otpService;
        _options = options.Value;
        _otpListState = otpListState;
        _credentialListState = credentialListState;
        _navigationService = navigationService;
        CloseCommand = new Command(async () =>
        {
            await _navigationService.GoBack();
        });
        ScanQRCodeCommand = new Command<BarcodeDetectionEventArgs>(async (c) =>
        {
            if (c == null || c.Results == null || !c.Results.Any() || IsLoading) return;
            var firstResult = c.Results.First().Value;
            await ScanQRCode(firstResult);
        });
    }


    public event PropertyChangedEventHandler PropertyChanged;
    public ICommand CloseCommand { get; private set; }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    public void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected async Task ScanQRCode(string qrCodeValue)
    {
        if (IsLoading) return;
        await _lck.WaitAsync();
        IsLoading = true;
        try
        {
            if (string.IsNullOrWhiteSpace(qrCodeValue)) return;
            if (!await RegisterOTPCode())
            {
                var qrCodeResult = JsonSerializer.Deserialize<QRCodeResult>(qrCodeValue);
                if (qrCodeResult.Action == "register") await Register(qrCodeResult);
                else await Authenticate(qrCodeResult);
            }
        }
        catch
        {
            await _promptService.ShowAlert("Error", "An error occured while trying to parse the QR Code");
        }
        finally
        {
            IsLoading = false;
            _lck.Release();
        }

        #region Register

        async Task Register(QRCodeResult qrCodeResult)
        {
            var beginResult = await ReadRegisterQRCode(qrCodeResult);
            var attestationBuilder = new FIDOU2FAttestationBuilder();
            var rp = beginResult.CredentialCreateOptions.Rp.Id;
            var existingCredential = _credentialListState.CredentialRecords.SingleOrDefault(r => r.Credential.Rp == rp && r.Credential.Login == beginResult.Login);
            if(existingCredential != null)
            {
                await _promptService.ShowAlert("Error", "The Credential has already been enrolled");
                return;
            }

            var enrollResponse = attestationBuilder.BuildEnrollResponse(new EnrollParameter
            {
                Challenge = beginResult.CredentialCreateOptions.Challenge,
                Rp = rp,
                Origin = qrCodeResult.GetOrigin()
            });
#if IOS && HOTRESTART == true
            await _promptService.ShowAlert("Error", "Host restart cannot be enabled");
            return;
#endif
            var endRegisterResult = await EndRegister(beginResult, enrollResponse);

            var credentialRecord = new CredentialRecord(enrollResponse.CredentialId, enrollResponse.AttestationCertificate.AttestationCertificate, enrollResponse.AttestationCertificate.PrivateKey, endRegisterResult.SignCount, rp, beginResult.Login);
            await _credentialListState.AddCredentialRecord(credentialRecord);
            await _promptService.ShowAlert("Success", "Your mobile device has been enrolled");
            await _navigationService.GoBack();
        }

        async Task<BeginU2FRegisterResult> ReadRegisterQRCode(QRCodeResult qrCodeResult)
        {
            var handler = new HttpClientHandler();
            if (_options.IgnoreHttps) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };
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

        async Task<EndU2FRegisterResult> EndRegister(BeginU2FRegisterResult beginResult, EnrollResult enrollResponse)
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
                var json = await httpResponse.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<EndU2FRegisterResult>(json);
            }
        }

        #endregion

        #region Authenticate

        async Task Authenticate(QRCodeResult qrCodeResult)
        {
            var credentialRecords = _credentialListState.CredentialRecords.ToList();
            var beginResult = await ReadAuthenticateQRCode(qrCodeResult);
            var attestationBuilder = new FIDOU2FAttestationBuilder();
            var allowCredentials = beginResult.Assertion.AllowCredentials;
            var selectedCredential = credentialRecords.SingleOrDefault(c => allowCredentials.Any(ac => ac.Id.SequenceEqual(c.Credential.IdPayload)))?.Credential;
            if(selectedCredential == null)
            {
                await _promptService.ShowAlert("Error", "Impossible to perform the authentication because you don't have the credential");
                return;
            }

            var authResponse = attestationBuilder.BuildAuthResponse(new AuthenticationParameter
            {
                Challenge = beginResult.Assertion.Challenge,
                Rp = beginResult.Assertion.RpId,
                Certificate = new AttestationCertificateResult(selectedCredential.Certificate, selectedCredential.PrivateKey),
                CredentialId = selectedCredential.IdPayload,
                Signcount = selectedCredential.SigCount,
                Origin = qrCodeResult.GetOrigin()
            });
            await EndAuthenticate(beginResult, authResponse);
            selectedCredential.SigCount++;
            await App.Database.UpdateCredentialRecord(selectedCredential);
            await _promptService.ShowAlert("Success", "You are authenticated");
            await _navigationService.GoBack();
        }

        async Task<BeginU2FAuthenticateResult> ReadAuthenticateQRCode(QRCodeResult qrCodeResult)
        {
            var handler = new HttpClientHandler();
            if (_options.IgnoreHttps) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };
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
                return JsonSerializer.Deserialize<BeginU2FAuthenticateResult>(json);
            }
        }

        async Task EndAuthenticate(BeginU2FAuthenticateResult beginAuthenticate, AuthenticatorAssertionRawResponse assertion)
        {
            var handler = new HttpClientHandler();
            if (_options.IgnoreHttps) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };
            using (var httpClient = new HttpClient(handler))
            {
                var deviceInfo = DeviceInfo.Current;
                var endLoginRequest = new EndU2FLoginRequest
                {
                    Login = beginAuthenticate.Login,
                    Assertion = assertion,
                    SessionId = beginAuthenticate.SessionId
                };
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(SanitizeEndRegisterUrl(beginAuthenticate.EndLoginUrl)),
                    Content = new StringContent(JsonSerializer.Serialize(endLoginRequest), Encoding.UTF8, "application/json")
                };
                var httpResponse = await httpClient.SendAsync(requestMessage);
                httpResponse.EnsureSuccessStatusCode();
            }
        }

        #endregion

        #region Register OTP Code

        async Task<bool> RegisterOTPCode()
        {
            if (_otpService.TryParse(qrCodeValue, out OTPCode otpCode))
            {
                var otpCodes = _otpListState.OTPCodes;
                var existingOtpCode = otpCodes.FirstOrDefault(o => o.Id == otpCode.Id);
                if(existingOtpCode != null)
                {
                    await _promptService.ShowAlert("Error", "The One Time Password has already been enrolled");
                    return true;
                }

                await _otpListState.AddOTPCode(otpCode);
                await _promptService.ShowAlert("Success", "The One Time Password has been enrolled");
                await _navigationService.GoBack();
                return true;
            }

            return false;
        }

        #endregion

        string SanitizeEndRegisterUrl(string url) => _options.IsDev ? url.Replace("localhost", "192.168.50.250") : url;
    }

    public ICommand ScanQRCodeCommand { get; private set; }
}
