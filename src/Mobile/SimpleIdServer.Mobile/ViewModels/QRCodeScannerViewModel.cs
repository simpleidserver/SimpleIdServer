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
using System.Text.Json.Nodes;
using System.Web;
using System.Windows.Input;
using ZXing.Net.Maui;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vp;
using SimpleIdServer.Vc;
using SimpleIdServer.Did.Key;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;
using Comet.Reflection;
#if IOS
using Firebase.CloudMessaging;
#endif

namespace SimpleIdServer.Mobile.ViewModels;

public class QRCodeScannerViewModel
{
    private const string _vpFormat = "ldp_vp";
    private const string _vcFormat = "ldp_vc";
    private const string openidCredentialOfferScheme = "openid-credential-offer://?credential_offer=";
    private const string openidVpScheme = "openid4vp://authorize?";
    private bool _isLoading = false;
    private readonly IPromptService _promptService;
    private readonly IOTPService _otpService;
    private readonly INavigationService _navigationService;
    private readonly IUrlService _urlService;
    private readonly Factories.IHttpClientFactory _httpClientFactory;
    private readonly OtpListState _otpListState;
    private readonly CredentialListState _credentialListState;
    private readonly VerifiableCredentialListState _verifiableCredentialListState;
    private readonly MobileSettingsState _mobileSettingsState;
    private readonly DidRecordState _didState;
    private readonly MobileOptions _options;
    private SemaphoreSlim _lck = new SemaphoreSlim(1, 1);

    public QRCodeScannerViewModel(
        IPromptService promptService,
        IOTPService otpService,
        INavigationService navigationService,
        IUrlService urlService,
        Factories.IHttpClientFactory httpClientFactory,
        OtpListState otpListState,
        CredentialListState credentialListState,
        VerifiableCredentialListState verifiableCredentialListState,
        MobileSettingsState mobileSettingsState,
        DidRecordState didState,
        IOptions<MobileOptions> options)
    {
        _promptService = promptService;
        _otpService = otpService;
        _urlService = urlService;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _otpListState = otpListState;
        _credentialListState = credentialListState;
        _verifiableCredentialListState = verifiableCredentialListState;
        _didState = didState;
        _mobileSettingsState = mobileSettingsState;
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
            var mobileSettings = _mobileSettingsState.Settings;
            if (!await RegisterOTPCode())
            {
                if(qrCodeValue.StartsWith(openidCredentialOfferScheme))
                {
                    await RegisterVerifiableCredential();
                }
                else if (qrCodeValue.StartsWith(openidVpScheme))
                {
                    await SendVerifiablePresentation();
                }
                else
                {
                    var qrCodeResult = JsonSerializer.Deserialize<QRCodeResult>(qrCodeValue);
                    if (qrCodeResult.Action == "register") await Register(qrCodeResult, mobileSettings);
                    else await Authenticate(qrCodeResult);
                }
            }
        }
        catch(Exception ex)
        {
            await _promptService.ShowAlert("Error", ex.ToString());
            await _promptService.ShowAlert("Error", "An error occured while trying to parse the QR Code");
        }
        finally
        {
            IsLoading = false;
            _lck.Release();
        }

        #region Register

        async Task Register(QRCodeResult qrCodeResult, MobileSettings mobileSettings)
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
            if(mobileSettings.NotificationMode == "firebase")
            {
                await _promptService.ShowAlert("Error", "Host restart cannot be enabled");
                return;
            }
#endif
            var endRegisterResult = await EndRegister(beginResult, enrollResponse, mobileSettings);

            var credentialRecord = new CredentialRecord(enrollResponse.CredentialId, enrollResponse.AttestationCertificate.AttestationCertificate, enrollResponse.AttestationCertificate.PrivateKey, endRegisterResult.SignCount, rp, beginResult.Login);
            await _credentialListState.AddCredentialRecord(credentialRecord);
            await _promptService.ShowAlert("Success", "Your mobile device has been enrolled");
            await _navigationService.GoBack();
        }

        async Task<BeginU2FRegisterResult> ReadRegisterQRCode(QRCodeResult qrCodeResult)
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_urlService.GetUrl(qrCodeResult.ReadQRCodeURL))
                };
                var httpResponse = await httpClient.SendAsync(requestMessage);
                httpResponse.EnsureSuccessStatusCode();
                var json = await httpResponse.Content.ReadAsStringAsync();
                var jObj = JsonObject.Parse(json);
                var credentialCreateOptionsJson = jObj["credential_create_options"].ToString();
                var result = new BeginU2FRegisterResult
                {
                    SessionId = jObj["session_id"].ToString(),
                    Login = jObj["login"].ToString(),
                    EndRegisterUrl = jObj["end_register_url"].ToString(),
                    CredentialCreateOptions = CredentialCreateOptions.FromJson(credentialCreateOptionsJson)
                };
                return result;
            }
        }

        async Task<EndU2FRegisterResult> EndRegister(BeginU2FRegisterResult beginResult, EnrollResult enrollResponse, MobileSettings mobileSettings)
        {
            var pushToken = mobileSettings.GotifyPushToken;
            if (mobileSettings.NotificationMode == "firebase")
            {
                pushToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            }

            using (var httpClient = _httpClientFactory.Build())
            {
                var deviceInfo = DeviceInfo.Current;
                var deviceData = new Dictionary<string, string>
                {
                    { "device_type", deviceInfo.Platform.ToString() },
                    { "model", deviceInfo.Model },
                    { "manufacturer", deviceInfo.Manufacturer },
                    { "name", deviceInfo.Name },
                    { "version", deviceInfo.VersionString },
                    { "push_token", pushToken },
                    { "push_type", mobileSettings.NotificationMode }
                };
                var dic = new Dictionary<string, object>
                {
                    { "login", beginResult.Login },
                    { "session_id", beginResult.SessionId },
                    { "attestation", ToJson(enrollResponse.Response) },
                    { "device_data", deviceData }
                };
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_urlService.GetUrl(beginResult.EndRegisterUrl)),
                    Content = new StringContent(JsonSerializer.Serialize(dic), Encoding.UTF8, "application/json")
                };
                var httpResponse = await httpClient.SendAsync(requestMessage);
                var json = await httpResponse.Content.ReadAsStringAsync();
                httpResponse.EnsureSuccessStatusCode();
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
            using (var httpClient = _httpClientFactory.Build())
            {
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_urlService.GetUrl(qrCodeResult.ReadQRCodeURL))
                };
                var httpResponse = await httpClient.SendAsync(requestMessage);
                httpResponse.EnsureSuccessStatusCode();
                var json = await httpResponse.Content.ReadAsStringAsync();
                var jObj = JsonObject.Parse(json);
                var assertionJson = jObj["assertion"].ToString();
                var result = new BeginU2FAuthenticateResult
                {
                    SessionId = jObj["session_id"].ToString(),
                    Login = jObj["login"].ToString(),
                    EndLoginUrl = jObj["end_login_url"].ToString(),
                    Assertion = AssertionOptions.FromJson(assertionJson)
                };
                return result;
            }
        }

        async Task EndAuthenticate(BeginU2FAuthenticateResult beginAuthenticate, AuthenticatorAssertionRawResponse assertion)
        {
            using (var httpClient = _httpClientFactory.Build())
            {
                var deviceInfo = DeviceInfo.Current;
                var endLoginRequest = new JsonObject
                {
                    { "login", beginAuthenticate.Login },
                    { "session_id", beginAuthenticate.SessionId },
                    { "assertion", ConvertToJson(assertion) }
                };
                var json = JsonSerializer.Serialize(assertion);
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_urlService.GetUrl(beginAuthenticate.EndLoginUrl)),
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

        #region Register verifiable credential

        async Task RegisterVerifiableCredential()
        {
            var serializedQueryParams = qrCodeValue.Replace(openidCredentialOfferScheme, string.Empty);
            var encodedJson = HttpUtility.UrlDecode(serializedQueryParams);
            var credentialOffer = JsonSerializer.Deserialize<CredentialOffer>(encodedJson);
            if (credentialOffer.CredentialConfigurationIds.Count() != 1)
            {
                await _promptService.ShowAlert("Error", "only one credential can be enrolled");
                return;
            }

            using (var httpClient = _httpClientFactory.Build())
            {
                var credentialDefinition = await GetCredentialDefinition(httpClient, credentialOffer);
                var serializedCredentialDef = credentialDefinition.CredentialsConfigurationsSupported
                    .Single(kvp => kvp.Key == credentialOffer.CredentialConfigurationIds.Single())
                    .Value.ToJsonString();
                var offeredCredential = JsonSerializer.Deserialize<CredentialDefinitionResult>(serializedCredentialDef);
                if (!offeredCredential.Display.Any())
                {
                    await _promptService.ShowAlert("Error", "credential cannot be enrolled because its definition doesn't contain display information");
                    return;
                }

                var display = offeredCredential.Display.First();
                var accessToken = await GetAccessTokenWithPreauthCode(credentialOffer, credentialDefinition, httpClient);
                if(string.IsNullOrWhiteSpace(accessToken))
                {
                    await _promptService.ShowAlert("Error", "the credential offer is expired or has been processed");
                    return;
                }

                var credentialResult = await GetCredential(httpClient, accessToken, credentialOffer, offeredCredential);
                var serializedVc = credentialResult.Credential.ToJsonString();
                var w3cVc = JsonSerializer.Deserialize<W3CVerifiableCredential>(serializedVc);
                var types = w3cVc.Type;
                types.Remove("VerifiableCredential");
                await _verifiableCredentialListState.AddVerifiableCredentialRecord(new VerifiableCredentialRecord
                {
                    Id = w3cVc.Id,
                    Format = _vcFormat,
                    Name = display.Name,
                    Description = display.Description,
                    ValidFrom = w3cVc.ValidFrom,
                    ValidUntil = w3cVc.ValidUntil,
                    Type = types.First(),
                    SerializedVc = serializedVc,
                    BackgroundColor = display.BackgroundColor,
                    TextColor = display.TextColor,
                    Logo = display.Logo?.Uri
                });
                await _promptService.ShowAlert("Success", "The verifiable credential has been enrolled");
            }
        }

        async Task<CredentialIssuerResult> GetCredentialDefinition(HttpClient httpClient, CredentialOffer credentialOffer)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_urlService.GetUrl($"{credentialOffer.CredentialIssuer}/.well-known/openid-credential-issuer"))
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CredentialIssuerResult>(json);
        }

        async Task<string> GetAccessTokenWithPreauthCode(CredentialOffer credentialOffer, CredentialIssuerResult credentialIssuer, HttpClient httpClient)
        {
            try
            {
                var dic = new Dictionary<string, string>
                {
                    { "grant_type", "urn:ietf:params:oauth:grant-type:pre-authorized_code" },
                    { "client_id", _options.ClientId },
                    { "client_secret", _options.ClientSecret },
                    { "pre-authorized_code", credentialOffer.Grants.PreAuthorizedCodeGrant.PreAuthorizedCode }
                };
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(dic),
                    RequestUri = new Uri(_urlService.GetUrl($"{credentialIssuer.AuthorizationServers.First()}/token"))
                };
                var httpResult = await httpClient.SendAsync(requestMessage);
                httpResult.EnsureSuccessStatusCode();
                var json = await httpResult.Content.ReadAsStringAsync();
                var accessToken = JsonObject.Parse(json).AsObject()["access_token"];
                return accessToken.ToString();
            }
            catch
            {
                return null;
            }
        }

        async Task<CredentialResult> GetCredential(HttpClient httpClient, string accessToken, CredentialOffer credentialOffer, CredentialDefinitionResult credentialDefinition)
        {
            var credentialRequest = new CredentialRequest
            {
                Format = _vcFormat,
                CredentialDefinitionRequest = new CredentialDefinitionRequest
                {
                    Type = credentialDefinition.Type
                }
            };
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_urlService.GetUrl($"{credentialOffer.CredentialIssuer}/credential")),
                Content = new StringContent(JsonSerializer.Serialize(credentialRequest), Encoding.UTF8, "application/json")
            };
            requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            var credentialResult = JsonSerializer.Deserialize<CredentialResult>(json);
            return credentialResult;
        }

        #endregion

        #region Verifiable presentation

        async Task SendVerifiablePresentation()
        {
            var didRecord = _didState.Did;
            if(didRecord == null)
            {
                return;
            }

            var didDocument = await DidKeyResolver.New().Resolve(didRecord.Did, CancellationToken.None);
            var privateKey = Ed25519SignatureKey.From(null, didRecord.PrivaterKey);
            var vcLst = _verifiableCredentialListState.VerifiableCredentialRecords;
            var serializedQueryParams = qrCodeValue.Replace(openidVpScheme, string.Empty);
            var encodedJson = HttpUtility.UrlDecode(serializedQueryParams);
            var vpAuthorizationRequest = JsonSerializer.Deserialize<VpAuthorizationRequest>(encodedJson);
            using (var httpClient = _httpClientFactory.Build())
            {
                var presentationDefinition = await GetPresentationDefinition(vpAuthorizationRequest, httpClient);
                var types = presentationDefinition.InputDescriptors.Select(d => d.Constraints).SelectMany(c => c.Fields).SelectMany(c => c.Path);
                var filteredVc = vcLst.Where(v => types.Contains(v.Type)).Select(v => JsonSerializer.Deserialize<W3CVerifiableCredential>(v.SerializedVc));
                var vpToken = await BuildVpToken(filteredVc, didDocument, privateKey);
                var presentationSubmission = BuildPresentationSubmission(presentationDefinition);
                var vpAuthorizationResponse = new VpAuthorizationResponse
                {
                    PresentationSubmission = JsonSerializer.Serialize(presentationSubmission),
                    State = vpAuthorizationRequest.State,
                    VpToken = vpToken
                };                
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_urlService.GetUrl(vpAuthorizationRequest.ResponseUri)),
                    Content = new FormUrlEncodedContent(vpAuthorizationResponse.ToQueries())
                };
                var httpResponse = await httpClient.SendAsync(requestMessage);
                httpResponse.EnsureSuccessStatusCode();
                await _promptService.ShowAlert("Success", "The verifiable presentation has been presented to the verifier");
            }
        }

        async Task<PresentationDefinitionResult> GetPresentationDefinition(VpAuthorizationRequest request, HttpClient httpClient)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_urlService.GetUrl(request.PresentationDefinitionUri))
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PresentationDefinitionResult>(json);
        }

        async Task<string> BuildVpToken(IEnumerable<W3CVerifiableCredential> filteredVc, DidDocument didDocument, Ed25519SignatureKey privateKey)
        {
            var builder = VpBuilder.New(Guid.NewGuid().ToString(), didDocument.Id);
            foreach (var vc in filteredVc)
                builder.AddVerifiableCredential(vc);

            var presentation = builder.Build();
            var securedDocument = SecuredDocument.New();
            securedDocument.Secure(
                presentation,
                didDocument,
                didDocument.VerificationMethod.First().Id,
                asymKey: privateKey);
            var vpToken = JsonSerializer.Serialize(presentation);
            return vpToken;
        }

        PresentationSubmissionRequest BuildPresentationSubmission(PresentationDefinitionResult presentationDefinition)
        {
            var result = new PresentationSubmissionRequest
            {
                Id = Guid.NewGuid().ToString(),
                DefinitionId = presentationDefinition.Id,
                DescriptorMap = new List<PresentationSubmissionDescriptorMapRequest>()
            };
            int i = 0;
            foreach(var d in presentationDefinition.InputDescriptors)
            {
                result.DescriptorMap.Add(new PresentationSubmissionDescriptorMapRequest
                {
                    Id = d.Id,
                    Format = _vpFormat,
                    Path = "$",
                    PathNested = new PresentationSubmissionDescriptorMapPathNestedRequest
                    {
                        Format = _vcFormat,
                        Path = $"$.verifiableCredential[{i}]"
                    }
                });
            }

            return result;
        }

        #endregion

        #region Serializer

        JsonObject ToJson(AuthenticatorAttestationRawResponse response)
        {
            var json = new JsonObject();
            if (response.Id != null)
                json.Add("id", Base64Url.Encode(response.Id));
            if (response.RawId != null)
                json.Add("rawId", Base64Url.Encode(response.RawId));
            if (response.Type == Fido2NetLib.Objects.PublicKeyCredentialType.PublicKey)
                json.Add("type", "public-key");
            if (response.Type == Fido2NetLib.Objects.PublicKeyCredentialType.Invalid)
                json.Add("type", "invalid");

            if (response.Response != null)
            {
                var responseJson = new JsonObject();
                if (response.Response.AttestationObject != null)
                    responseJson.Add("attestationObject", Base64Url.Encode(response.Response.AttestationObject));
                if (response.Response.ClientDataJson != null)
                    responseJson.Add("clientDataJSON", Base64Url.Encode(response.Response.ClientDataJson));
                json.Add("response", responseJson);
            }

            return json;
        }

        JsonObject ConvertToJson(AuthenticatorAssertionRawResponse response)
        {
            var json = new JsonObject();
            if (response.Id != null)
                json.Add("id", Base64Url.Encode(response.Id));
            if (response.RawId != null)
                json.Add("rawId", Base64Url.Encode(response.RawId));
            if (response.Type == Fido2NetLib.Objects.PublicKeyCredentialType.PublicKey)
                json.Add("type", "public-key");
            if (response.Type == Fido2NetLib.Objects.PublicKeyCredentialType.Invalid)
                json.Add("type", "invalid");
            if (response.Response != null)
            {
                var responseJson = new JsonObject();
                if (response.Response.AuthenticatorData != null)
                    responseJson.Add("authenticatorData", Base64Url.Encode(response.Response.AuthenticatorData));
                if (response.Response.Signature != null)
                    responseJson.Add("signature", Base64Url.Encode(response.Response.Signature));
                if (response.Response.ClientDataJson != null)
                    responseJson.Add("clientDataJSON", Base64Url.Encode(response.Response.ClientDataJson));
                if (response.Response.UserHandle != null)
                    responseJson.Add("userHandle", Base64Url.Encode(response.Response.UserHandle));
                json.Add("response", responseJson);
            }

            return json;
        }

        #endregion
    }

    public ICommand ScanQRCodeCommand { get; private set; }
}
