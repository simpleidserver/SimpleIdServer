// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using OpenId.AppAuth;

namespace ProtectAPIFromUndesirableUsers.Native
{
    [Activity(Label = "SimpleIdServer.XamarinMobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private AuthorizationService authService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            authService = new AuthorizationService(this);
            var idpButton = FindViewById<Button>(Resource.Id.button1);
            idpButton.Click += async delegate
            {
                AuthorizationServiceConfiguration serviceConfiguration = new AuthorizationServiceConfiguration(
                        Android.Net.Uri.Parse($"{NativeConstants.OPENID_BASE_URL}/authorization"),
                        Android.Net.Uri.Parse($"{NativeConstants.OPENID_BASE_URL}/token"),
                        Android.Net.Uri.Parse($"{NativeConstants.OPENID_BASE_URL}/registration"));
                MakeAuthRequest(serviceConfiguration, new AuthState());
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            authService.Dispose();
        }

        private void MakeAuthRequest(AuthorizationServiceConfiguration serviceConfig, AuthState authState)
        {
            var authRequest = new AuthorizationRequest.Builder(serviceConfig, NativeConstants.CLIENT_ID, ResponseTypeValues.Code, Android.Net.Uri.Parse(NativeConstants.REDIRECT_URI))
                .SetScope("openid profile")
                .Build();            
            authService.PerformAuthorizationRequest(
                authRequest,
                TokenActivity.CreatePostAuthorizationIntent(this, authRequest, serviceConfig.DiscoveryDoc, authState));
        }
    }
}