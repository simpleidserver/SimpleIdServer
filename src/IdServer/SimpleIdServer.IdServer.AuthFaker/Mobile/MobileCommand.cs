// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;
using SimpleIdServer.IdServer.AuthFaker.Mobile;
using SimpleIdServer.IdServer.AuthFaker.Stores;
using SimpleIdServer.IdServer.U2FClient;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.AuthFaker.VpRegistration;

public class MobileCommand
{
    public async Task Execute(QRCodeResult qrCodeResult)
    {
        if (qrCodeResult.Action == "register") await Register(qrCodeResult);
        else await Authenticate(qrCodeResult);
    }

    #region Register

    private async Task Register(QRCodeResult qrCodeResult)
    {
        var beginRegisterResult = await BeginRegister(qrCodeResult);
        var enrollResponse = BuildEnroll(beginRegisterResult, qrCodeResult);
        var endRegister = await EndRegister(beginRegisterResult, enrollResponse);
        var credentialRecord = new CredentialRecord(enrollResponse.CredentialId, enrollResponse.AttestationCertificate.AttestationCertificate, enrollResponse.AttestationCertificate.PrivateKey, endRegister.SignCount, beginRegisterResult.CredentialCreateOptions.Rp.Id, beginRegisterResult.Login);
        CredentialStorage.New().Update(credentialRecord);
    }

    private async Task<BeginU2FRegisterResult> BeginRegister(QRCodeResult qrCodeResult)
    {
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(qrCodeResult.ReadQRCodeURL);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var jObj = JsonNode.Parse(json);
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

    private EnrollResult BuildEnroll(BeginU2FRegisterResult beginResult, QRCodeResult qrCodeResult)
    {
        var attestationBuilder = new FIDOU2FAttestationBuilder();
        var enrollResponse = attestationBuilder.BuildEnrollResponse(new EnrollParameter
        {
            Challenge = beginResult.CredentialCreateOptions.Challenge,
            Rp = beginResult.CredentialCreateOptions.Rp.Id,
            Origin = qrCodeResult.GetOrigin()
        });
        return enrollResponse;
    }

    private async Task<EndU2FRegisterResult> EndRegister(BeginU2FRegisterResult beginResult, EnrollResult enrollResponse)
    {
        using (var httpClient = new HttpClient())
        {
            var deviceData = new Dictionary<string, string>
            {
                { "device_type", "android" },
                { "model", "model" },
                { "manufacturer", "xaomi" },
                { "name", "POCOPHONE" },
                { "version", "5.0" },
                { "push_token", "push_token" },
                { "push_type", "GOTIFY" }
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
                RequestUri = new Uri(beginResult.EndRegisterUrl),
                Content = new StringContent(JsonSerializer.Serialize(dic), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(requestMessage);
            var json = await httpResult.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EndU2FRegisterResult>(json);
        }
    }

    #endregion

    #region Authenticate

    private async Task Authenticate(QRCodeResult qrCodeResult)
    {
        var beginResult = await BeginAuthenticate(qrCodeResult);
        var credential = CredentialStorage.New().Get();
        var authResponse = BuildAttestation(qrCodeResult, beginResult, credential);
        await EndAuthenticate(beginResult, authResponse);
        credential.SigCount++;
        CredentialStorage.New().Update(credential);
    }

    private async Task<BeginU2FAuthenticateResult> BeginAuthenticate(QRCodeResult qrCodeResult)
    {
        using (var httpClient = new HttpClient())
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(qrCodeResult.ReadQRCodeURL)
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

    private AuthenticatorAssertionRawResponse BuildAttestation(QRCodeResult qrCodeResult, BeginU2FAuthenticateResult beginResult, CredentialRecord credential)
    {
        var attestationBuilder = new FIDOU2FAttestationBuilder();
        var allowCredentials = beginResult.Assertion.AllowCredentials;
        var authResponse = attestationBuilder.BuildAuthResponse(new AuthenticationParameter
        {
            Challenge = beginResult.Assertion.Challenge,
            Rp = beginResult.Assertion.RpId,
            Certificate = new AttestationCertificateResult(credential.Certificate, credential.PrivateKey),
            CredentialId = credential.IdPayload,
            Signcount = credential.SigCount,
            Origin = qrCodeResult.GetOrigin()
        });
        return authResponse;
    }
    
    private async Task EndAuthenticate(BeginU2FAuthenticateResult beginAuthenticate, AuthenticatorAssertionRawResponse assertion)
    {
        using (var httpClient = new HttpClient())
        {
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
                RequestUri = new Uri(beginAuthenticate.EndLoginUrl),
                Content = new StringContent(JsonSerializer.Serialize(endLoginRequest), Encoding.UTF8, "application/json")
            };
            await httpClient.SendAsync(requestMessage);
        }
    }

    #endregion

    private static JsonObject ToJson(AuthenticatorAttestationRawResponse response)
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

    private static JsonObject ConvertToJson(AuthenticatorAssertionRawResponse response)
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
}
