// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    public static class TokenRequestParameters
    {
        public const string GrantType = "grant_type";
        public const string Username = "username";
        public const string Password = "password";
        public const string Scope = "scope";
        public const string Code = "code";
        public const string RedirectUri = "redirect_uri";
        public const string RefreshToken = "refresh_token";
        public const string CodeVerifier = "code_verifier";
        public const string AmrValues = "amr_values";
        public const string ClientAssertion = "client_assertion";
        public const string ClientAssertionType = "client_assertion_type";
        public const string ClientId = "client_id";
        public const string ClientSecret = "client_secret";
        public const string Ticket = "ticket";
        public const string ClaimToken = "claim_token";
        public const string ClaimTokenFormat = "claim_token_format";
        public const string Pct = "pct";
        public const string Rpt = "rpt";
        public const string DeviceCode = "device_code";
    }
}