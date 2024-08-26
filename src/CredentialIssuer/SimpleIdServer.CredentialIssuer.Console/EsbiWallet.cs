// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.CredentialIssuer.Api.CredentialIssuer;
using SimpleIdServer.Did.Crypto;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace SimpleIdServer.CredentialIssuer.Console;

public class EsbiWallet
{
    private const string url = "https://api-conformance.ebsi.eu/conformance/v3/issuer-mock/.well-known/openid-credential-issuer";
    private const string publicKey = "z2dmzD81cgPx8Vki7JbuuMmFYrWPgYoytykUZ3eyqht1j9KbpMAoXtZtunruYnM4gCV65AKAUX2AwEReRhEaf3BRQNJArZPwQdmf9ENZcF8VT13a58WsHeVjJtvAKKPYEibaEfdUxvU7sgxEUTJpjEkq6BJKrRV1JQ1CqhYvGbmJ1WyoUQ";
    private const string did = $"did:key:{publicKey}";
    private const string refDid = $"{did}#{publicKey}";
    // private const string credentialType = "CTWalletSameAuthorisedInTime";
    private const string credentialType = "CTIssueQualificationCredential";

    public static async Task RegisterEsbiWalletForConformance()
    {
        using (var httpClient = new HttpClient())
        {
            var (challenge, verifier) = PkceGenerate();
            var openidCredentialIssuer = GetOpenidCredentialIssuer(httpClient).Result;
            var configuration = GetAuthorizationEndpoint(httpClient, openidCredentialIssuer.AuthorizationServer).Result;
            var parameters = ExecuteAuthorizationRequest(httpClient, configuration.authorizationEndpoint, challenge, openidCredentialIssuer.CredentialIssuer).Result;
            var redirectUri = parameters["redirect_uri"];
            var nonce = parameters["nonce"];
            var state = parameters["state"];
            var postAuthResult = ExecutePostAuthorizationRequest(httpClient, redirectUri, openidCredentialIssuer.CredentialIssuer, nonce, state).Result;
            var tokenResult = GetToken(httpClient, configuration.tokenEndpoint, postAuthResult["code"], verifier).Result;
            GetCredential(httpClient, openidCredentialIssuer.CredentialEndpoint, openidCredentialIssuer.CredentialIssuer, tokenResult.cNonce, tokenResult.accessToken).Wait();
        }
    }

    private static async Task<ESBICredentialIssuerResult> GetOpenidCredentialIssuer(HttpClient httpClient)
    {
        var url = "https://api-conformance.ebsi.eu/conformance/v3/issuer-mock/.well-known/openid-credential-issuer";
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var openidCredentialIssuer = JsonSerializer.Deserialize<ESBICredentialIssuerResult>(json);
        return openidCredentialIssuer;
    }

    private static async Task<(string authorizationEndpoint, string issuer, string tokenEndpoint)> GetAuthorizationEndpoint(HttpClient httpClient, string authUrl)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{authUrl}/.well-known/openid-configuration");
        var httpResult = await httpClient.SendAsync(requestMessage);
        var json = await httpResult.Content.ReadAsStringAsync();
        var jsonObj = JsonObject.Parse(json);
        return (jsonObj["authorization_endpoint"].ToString(), jsonObj["issuer"].ToString(), jsonObj["token_endpoint"].ToString());
    }

    private static async Task<Dictionary<string, string>> ExecuteAuthorizationRequest(HttpClient httpClient, string url, string challenge, string aud)
    {
        var uriBuilder = new UriBuilder(url);
        var authorizationDetails = new JsonArray
{
    new JsonObject
    {
        { "type", "openid_credential" },
        { "format", "jwt_vc" },
        { "types", new JsonArray
        {
            "VerifiableCredential",
            "VerifiableAttestation",
            credentialType
        } },
        { "locations", new JsonArray
        {
            aud
        } }
    }
};
        var clientMetadata = new JsonObject
{
    {  "response_types_supported",
        new JsonArray
        {
            "vp_token", "id_token"
        }
    },
    {  "authorization_endpoint", "openid://"}
};
        var dic = new Dictionary<string, string>
{
    { "response_type", "code" },
    { "scope", "openid" },
    // { "issuer_state", "issuer-state" },
    { "state", "client-state" },
    { "client_id", did },
    { "authorization_details", HttpUtility.UrlEncode(authorizationDetails.ToJsonString()) },
    { "redirect_uri", "openid://" },
    { "nonce", "nonce" },
    { "code_challenge", challenge },
    { "code_challenge_method", "S256" },
    { "client_metadata", HttpUtility.UrlEncode(clientMetadata.ToJsonString()) }
};
        uriBuilder.Query = string.Join("&", dic.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = uriBuilder.Uri
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var result = httpResult.Headers.Location.AbsoluteUri;
        uriBuilder = new UriBuilder(result);
        var queryParameters = uriBuilder.Query.Trim('?').Split('&').Select(s => s.Split('=')).ToDictionary(arr => arr[0], arr => arr[1]);
        return queryParameters;
    }

    private static async Task<Dictionary<string, string>> ExecutePostAuthorizationRequest(HttpClient httpClient, string url, string aud, string nonce, string state)
    {
        var serializedPrivateKey = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "privatekey.json"));
        var signatureKey = SignatureKeySerializer.Deserialize(serializedPrivateKey);
        var signingCredentials = signatureKey.BuildSigningCredentials(refDid);
        var handler = new JsonWebTokenHandler();
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = signingCredentials,
            Audience = aud
        };
        var claims = new Dictionary<string, object>
{
    { "nonce", nonce },
    { "iss", did },
    { "sub", did }
};
        securityTokenDescriptor.Claims = claims;
        var token = handler.CreateToken(securityTokenDescriptor);
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(HttpUtility.UrlDecode(url)),
            Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
    {
        new KeyValuePair<string, string>("id_token", token),
        new KeyValuePair<string, string>("state", state)
    }),
            Method = HttpMethod.Post
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var result = httpResult.Headers.Location.AbsoluteUri;
        var uriBuilder = new UriBuilder(result);
        var queryParameters = uriBuilder.Query.Trim('?').Split('&').Select(s => s.Split('=')).ToDictionary(arr => arr[0], arr => arr[1]);
        return queryParameters;
    }

    private static async Task GetCredential(HttpClient httpClient, string credentialEndpoint, string aud, string nonce, string accessToken)
    {
        var serializedPrivateKey = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "privatekey.json"));
        var signatureKey = SignatureKeySerializer.Deserialize(serializedPrivateKey);
        var signingCredentials = signatureKey.BuildSigningCredentials(refDid);
        var handler = new JsonWebTokenHandler();
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = signingCredentials,
            Audience = aud,
            TokenType = "openid4vci-proof+jwt"
        };
        var claims = new Dictionary<string, object>
{
    { "iss", did },
    { "nonce", nonce }
};
        securityTokenDescriptor.Claims = claims;
        var proofJwt = handler.CreateToken(securityTokenDescriptor);
        var proofRequest = new JsonObject();
        proofRequest.Add("proof_type", "jwt");
        proofRequest.Add("jwt", proofJwt);
        var request = new JsonObject
{
    { "types",  new JsonArray
        {
            "VerifiableCredential",
            "VerifiableAttestation",
            credentialType
        } },
    { "format", "jwt_vc" }
};
        request.Add("proof", proofRequest);
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(HttpUtility.UrlDecode(credentialEndpoint)),
            Content = new StringContent(request.ToJsonString(), Encoding.UTF8, "application/json"),
            Method = HttpMethod.Post
        };
        requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
        var httpResult = await httpClient.SendAsync(requestMessage);
        var content = await httpResult.Content.ReadAsStringAsync();
        System.Console.WriteLine(content);
    }

    private static async Task<(string accessToken, string idToken, string cNonce)> GetToken(HttpClient httpClient, string url, string authorizationCode, string codeVerifier)
    {
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
    {
        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        new KeyValuePair<string, string>("client_id", did),
        new KeyValuePair<string, string>("code", authorizationCode),
        new KeyValuePair<string, string>("code_verifier", codeVerifier)
    }),
            Method = HttpMethod.Post
        };
        var httpResult = await httpClient.SendAsync(requestMessage);
        var content = await httpResult.Content.ReadAsStringAsync();
        var jObj = JsonObject.Parse(content);
        return (jObj["access_token"].ToString(), jObj["id_token"].ToString(), jObj["c_nonce"].ToString());
    }

    private static (string codeChallenge, string verifier) PkceGenerate(int size = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[size];
        rng.GetBytes(randomBytes);
        var verifier = Base64UrlEncode(randomBytes);

        var buffer = Encoding.UTF8.GetBytes(verifier);
        var hash = SHA256.Create().ComputeHash(buffer);
        var challenge = Base64UrlEncode(hash);

        return (challenge, verifier);
    }
    private static string Base64UrlEncode(byte[] data) =>
            Convert.ToBase64String(data)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
}
