// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace SimpleIdServer.IdServer;

public static class Counters
{
    public static string ServiceName => typeof(Counters).Assembly.GetName().Name;

    public static string ServiceVersion => typeof(Counters).Assembly.GetName().Version.ToString();

    private static readonly Meter Meter = new Meter(ServiceName, ServiceVersion);

    public static class Names
    {
        public const string User = "user.count";
        public const string AuthRequest = "auth.requests.count";
        public const string AuthSuccess = "auth.success.count";
        public const string AuthFailure = "auth.failure.count";
        public const string Logout = "logout.count";
        public const string ConsentPrompt = "consent.prompt.count";
        public const string ConsentAccepted = "consent.accepted.count";
        public const string ConsentRejected = "consent.rejected.count";
        public const string TokenIssued = "token.issued.count";
        public const string TokenRejected = "token.rejected.count";
        public const string IdTokenIssued = "id_token.issued.count";
        public const string RefreshTokenIssued = "refresh_token.issued.count";
        public const string TokenFailed = "token.failed.count";
        public const string AccessTokenIssued = "access_token.issued.count";
        public const string AuthorizationCodeIssued = "authorization_code.issued.count";
    }

    public static class Tags
    {
        public const string Client = "client";
        public const string Realm = "realm";
        public const string GrantType = "grant_type";
    }

    private static Counter<long> UserCounter = Meter.CreateCounter<long>(Names.User);

    private static Counter<long> AuthRequestCounter = Meter.CreateCounter<long>(Names.AuthRequest);

    private static Counter<long> AuthSuccessCounter = Meter.CreateCounter<long>(Names.AuthSuccess);

    private static Counter<long> AuthFailureCounter = Meter.CreateCounter<long>(Names.AuthFailure);

    private static Counter<long> LogoutCounter = Meter.CreateCounter<long>(Names.Logout);

    private static Counter<long> ConsentPromptCounter = Meter.CreateCounter<long>(Names.ConsentPrompt);

    private static Counter<long> ConsentAcceptedCounter = Meter.CreateCounter<long>(Names.ConsentAccepted);

    private static Counter<long> ConsentRejectedCounter = Meter.CreateCounter<long>(Names.ConsentRejected);

    private static Counter<long> TokenIssuedCounter = Meter.CreateCounter<long>(Names.TokenIssued);

    private static Counter<long> IdTokenIssuedCounter = Meter.CreateCounter<long>(Names.IdTokenIssued);

    private static Counter<long> RefreshTokenIssuedCounter = Meter.CreateCounter<long>(Names.RefreshTokenIssued);

    private static Counter<long> AccessTokenIssuedCounter = Meter.CreateCounter<long>(Names.AccessTokenIssued);

    private static Counter<long> AuthorizationCodeIssuedCounter = Meter.CreateCounter<long>(Names.AuthorizationCodeIssued);

    private static Counter<long> TokenFailedCounter = Meter.CreateCounter<long>(Names.TokenFailed);

    private static Counter<long> TokenRejectedCounter = Meter.CreateCounter<long>(Names.TokenRejected);

    public static void UserRegistered()
    {
        UserCounter.Add(1);
    }

    public static void AuthSuccess(string clientId, string realm)
    {
        AuthRequestCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
        AuthSuccessCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
    }

    public static void AuthFailure(string clientId, string realm)
    {
        AuthRequestCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
        AuthFailureCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
    }

    public static void Logout(string realm)
    {
        LogoutCounter.Add(1, new KeyValuePair<string, object?>(Tags.Realm, realm));
    }

    public static void AcceptConsent(string clientId, string realm)
    {
        ConsentPromptCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
        ConsentAcceptedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
    }

    public static void RejectConsent(string clientId, string realm)
    {
        ConsentPromptCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
        ConsentRejectedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
    }

    public static void IssueIdToken(string clientId, string realm, string grantType)
    {
        TokenIssuedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm), new(Tags.GrantType, grantType));
        IdTokenIssuedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm), new(Tags.GrantType, grantType));
    }

    public static void IssueRefreshToken(string clientId, string realm, string grantType)
    {
        TokenIssuedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm), new(Tags.GrantType, grantType));
        RefreshTokenIssuedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm), new(Tags.GrantType, grantType));
    }

    public static void IssueAccessToken(string clientId, string realm, string grantType)
    {
        TokenIssuedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm), new(Tags.GrantType, grantType));
        AccessTokenIssuedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm), new(Tags.GrantType, grantType));
    }

    public static void FailToken(string clientId, string realm, string grantType)
    {
        TokenFailedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm), new(Tags.GrantType, grantType));
    }

    public static void RejectToken(string clientId, string realm)
    {
        TokenRejectedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
    }

    public static void IssueAuthorizationCode(string clientId, string realm)
    {
        AuthorizationCodeIssuedCounter.Add(1, new(Tags.Client, clientId), new(Tags.Realm, realm));
    }
}