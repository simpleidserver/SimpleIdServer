// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Javax.Net.Ssl;
using Newtonsoft.Json.Linq;
using OpenId.AppAuth;
using Org.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ProtectAPIFromUndesirableUsers.Native
{
    [Activity(Label = "TokenActivity")]
    public class TokenActivity : Activity
    {
        private static string KEY_AUTH_STATE = "authState";
        private static string KEY_USER_INFO = "userInfo";
        private static string EXTRA_AUTH_SERVICE_DISCOVERY = "authServiceDiscovery";
        private static string EXTRA_AUTH_STATE = "authState";
        private AuthState authState;
        private JSONObject userInfoJson;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_token);

            if (savedInstanceState != null)
            {
                if (savedInstanceState.ContainsKey(KEY_AUTH_STATE))
                {
                    authState = AuthState.JsonDeserialize(savedInstanceState.GetString(KEY_AUTH_STATE));
                }

                if (savedInstanceState.ContainsKey(KEY_USER_INFO))
                {
                    userInfoJson = new JSONObject(savedInstanceState.GetString(KEY_USER_INFO));
                }
            }

            if (authState == null)
            {
                authState = GetAuthStateFromIntent(Intent);
                AuthorizationResponse response = AuthorizationResponse.FromIntent(Intent);
                AuthorizationException ex = AuthorizationException.FromIntent(Intent);
                authState.Update(response, ex);
                if (response != null)
                {
                    PerformTokenRequest(response.CreateTokenExchangeRequest());
                }
            }
        }

        public static PendingIntent CreatePostAuthorizationIntent(Context context, AuthorizationRequest request, AuthorizationServiceDiscovery discoveryDoc, AuthState authState)
        {
            var intent = new Intent(context, typeof(TokenActivity));
            intent.PutExtra(EXTRA_AUTH_STATE, authState.JsonSerializeString());
            if (discoveryDoc != null)
            {
                intent.PutExtra(EXTRA_AUTH_SERVICE_DISCOVERY, discoveryDoc.DocJson.ToString());
            }

            return PendingIntent.GetActivity(context, request.GetHashCode(), intent, 0);
        }
                       
        private void PerformTokenRequest(TokenRequest tokenRequest)
        {
            var handler = new BypassSslValidationClientHandler();
            using (var httpClient = new HttpClient(handler, false))
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{NativeConstants.OPENID_BASE_URL}/token"),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", NativeConstants.CLIENT_ID },
                        { "grant_type", "authorization_code" },
                        { "code", tokenRequest.AuthorizationCode },
                        { "code_verifier", tokenRequest.CodeVerifier },
                        { "redirect_uri", NativeConstants.REDIRECT_URI },
                        { "scope", "openid profile" }
                    })
                };
                httpClient.SendAsync(request).ContinueWith((t) =>
                {
                    var httpResult = t.Result;
                    if (!httpResult.IsSuccessStatusCode)
                    {
                        return;
                    }

                    httpResult.Content.ReadAsStringAsync().ContinueWith((st) =>
                    {
                        var json = st.Result;
                        var jObj = JObject.Parse(json);
                        this.RunOnUiThread(() => { RefreshUi(jObj["id_token"].ToString(), jObj["access_token"].ToString()); });
                    });
                });
            }
        }

        private void ReceivedTokenResponse(TokenResponse tokenResponse, AuthorizationException authException)
        {
            authState.Update(tokenResponse, authException);
            RefreshUi(tokenResponse.IdToken, tokenResponse.AccessToken);
        }

        private void RefreshUi(string idToken, string accessToken)
        {
            var accessTokenInfoView = FindViewById<TextView>(Resource.Id.access_token_info);
            var idTokenInfoView = FindViewById<TextView>(Resource.Id.id_token_info);
            var at = $"access token : {accessToken}";
            var id = $"identity token : {idToken}";
            accessTokenInfoView.SetText(at.ToCharArray(), 0, at.Length - 1);
            idTokenInfoView.SetText(id.ToCharArray(), 0, id.Length - 1);
        }

        private static AuthState GetAuthStateFromIntent(Intent intent)
        {
            if (!intent.HasExtra(EXTRA_AUTH_STATE))
            {
                throw new InvalidOperationException("The AuthState instance is missing in the intent.");
            }

            try
            {
                return AuthState.JsonDeserialize(intent.GetStringExtra(EXTRA_AUTH_STATE));
            }
            catch (JSONException ex)
            {
                throw new InvalidOperationException("The AuthState instance is missing in the intent.", ex);
            }
        }

        private class BypassSslValidationClientHandler : Xamarin.Android.Net.AndroidClientHandler
        {
            protected override SSLSocketFactory ConfigureCustomSSLSocketFactory(HttpsURLConnection connection)
            {
                return Android.Net.SSLCertificateSocketFactory.GetInsecure(1000, null);
            }

            protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
            {
                return new BypassHostnameVerifier();
            }
        }

        private class BypassHostnameVerifier : Java.Lang.Object, IHostnameVerifier
        {
            public bool Verify(string hostname, ISSLSession session)
            {
                return true;
            }
        }
    }
}