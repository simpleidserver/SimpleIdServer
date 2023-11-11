// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Register
{
    [JsonConverter(typeof(TranslatableRequestConverter<RegisterClientRequest>))]
    public class RegisterClientRequest : ITranslatableRequest
    {
        [JsonPropertyName(OAuthClientParameters.ApplicationType)]
        [BindProperty(Name = OAuthClientParameters.ApplicationType)]
        public string? ApplicationType { get; set; }
        [BindProperty(Name = OAuthClientParameters.SoftwareStatement)]
        [JsonPropertyName(OAuthClientParameters.SoftwareStatement)]
        public string? SoftwareStatement { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.GrantTypes)]
        [JsonPropertyName(OAuthClientParameters.GrantTypes)]
        public IEnumerable<string> GrantTypes { get; set; } = new string[0];
        [BindProperty(Name = OAuthClientParameters.Scope)]
        [JsonPropertyName(OAuthClientParameters.Scope)]
        public string? Scope { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.TokenEndpointAuthMethod)]
        [JsonPropertyName(OAuthClientParameters.TokenEndpointAuthMethod)]
        public string? TokenAuthMethod { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.ResponseTypes)]
        [JsonPropertyName(OAuthClientParameters.ResponseTypes)]
        public ICollection<string> ResponseTypes { get; set; } = new string[0];
        [BindProperty(Name = OAuthClientParameters.TokenSignedResponseAlg)]
        [JsonPropertyName(OAuthClientParameters.TokenSignedResponseAlg)]
        public string? TokenSignedResponseAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.TokenEncryptedResponseAlg)]
        [JsonPropertyName(OAuthClientParameters.TokenEncryptedResponseAlg)]
        public string? TokenEncryptedResponseAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.IdTokenSignedResponseAlg)]
        [JsonPropertyName(OAuthClientParameters.IdTokenSignedResponseAlg)]
        public string? IdTokenSignedResponseAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.IdTokenEncryptedResponseEnc)]
        [JsonPropertyName(OAuthClientParameters.IdTokenEncryptedResponseEnc)]
        public string? IdTokenEncryptedResponseEnc { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.IdTokenEncryptedResponseAlg)]
        [JsonPropertyName(OAuthClientParameters.IdTokenEncryptedResponseAlg)]
        public string? IdTokenEncryptedResponseAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.UserInfoSignedResponseAlg)]
        [JsonPropertyName(OAuthClientParameters.UserInfoSignedResponseAlg)]
        public string? UserInfoSignedResponseAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.UserInfoEncryptedResponseAlg)]
        [JsonPropertyName(OAuthClientParameters.UserInfoEncryptedResponseAlg)]
        public string? UserInfoEncryptedResponseAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.UserInfoEncryptedResponseEnc)]
        [JsonPropertyName(OAuthClientParameters.UserInfoEncryptedResponseEnc)]
        public string? UserInfoEncryptedResponseEnc { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.RequestObjectSigningAlg)]
        [JsonPropertyName(OAuthClientParameters.RequestObjectSigningAlg)]
        public string? RequestObjectSigningAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.RequestObjectEncryptionEnc)]
        [JsonPropertyName(OAuthClientParameters.RequestObjectEncryptionEnc)]
        public string? RequestObjectEncryptionEnc { get; set; }
        [BindProperty(Name = OAuthClientParameters.RequestObjectEncryptionAlg)]
        [JsonPropertyName(OAuthClientParameters.RequestObjectEncryptionAlg)]
        public string? RequestObjectEncryptionAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.TokenEncryptedResponseEnc)]
        [JsonPropertyName(OAuthClientParameters.TokenEncryptedResponseEnc)]
        public string? TokenEncryptedResponseEnc { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.RedirectUris)]
        [JsonPropertyName(OAuthClientParameters.RedirectUris)]
        public ICollection<string> RedirectUris { get; set; } = new List<string>();
        [BindProperty(Name = OAuthClientParameters.JwksUri)]
        [JsonPropertyName(OAuthClientParameters.JwksUri)]
        public string? JwksUri { get; set; }
        [BindProperty(Name = OAuthClientParameters.SectorIdentifierUri)]
        [JsonPropertyName(OAuthClientParameters.SectorIdentifierUri)]
        public string? SectorIdentifierUri { get; set; }
        [BindProperty(Name = OAuthClientParameters.InitiateLoginUri)]
        [JsonPropertyName(OAuthClientParameters.InitiateLoginUri)]
        public string? InitiateLoginUri { get; set; }
        [BindProperty(Name = OAuthClientParameters.SubjectType)]
        [JsonPropertyName(OAuthClientParameters.SubjectType)]
        public string? SubjectType { get; set; }
        [BindProperty(Name = OAuthClientParameters.BCTokenDeliveryMode)]
        [JsonPropertyName(OAuthClientParameters.BCTokenDeliveryMode)]
        public string? BCTokenDeliveryMode { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.BCClientNotificationEndpoint)]
        [JsonPropertyName(OAuthClientParameters.BCClientNotificationEndpoint)]
        public string? BCClientNotificationEndpoint { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.BCAuthenticationRequestSigningAlg)]
        [JsonPropertyName(OAuthClientParameters.BCAuthenticationRequestSigningAlg)]
        public string? BCAuthenticationRequestSigningAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.DefaultMaxAge)]
        [JsonPropertyName(OAuthClientParameters.DefaultMaxAge)]
        public double? DefaultMaxAge { get; set; }
        [BindProperty(Name = OAuthClientParameters.BCUserCodeParameter)]
        [JsonPropertyName(OAuthClientParameters.BCUserCodeParameter)]
        public bool BCUserCodeParameter { get; set; } = false;
        [BindProperty(Name = OAuthClientParameters.FrontChannelLogoutUri)]
        [JsonPropertyName(OAuthClientParameters.FrontChannelLogoutUri)]
        public string? FrontChannelLogoutUri { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.FrontChannelLogoutSessionRequired)]
        [JsonPropertyName(OAuthClientParameters.FrontChannelLogoutSessionRequired)]
        public bool FrontChannelLogoutSessionRequired { get; set; } = false;
        [BindProperty(Name = OAuthClientParameters.BackChannelLogoutUri)]
        [JsonPropertyName(OAuthClientParameters.BackChannelLogoutUri)]
        public string? BackChannelLogoutUri { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.BackChannelLogoutSessionRequired)]
        [JsonPropertyName(OAuthClientParameters.BackChannelLogoutSessionRequired)]
        public bool BackChannelLogoutSessionRequired { get; set; } = false;
        [BindProperty(Name = OAuthClientParameters.DefaultAcrValues)]
        [JsonPropertyName(OAuthClientParameters.DefaultAcrValues)]
        public ICollection<string> DefaultAcrValues { get; set; } = new string[0];
        [BindProperty(Name = OAuthClientParameters.PostLogoutRedirectUris)]
        [JsonPropertyName(OAuthClientParameters.PostLogoutRedirectUris)]
        public ICollection<string> PostLogoutRedirectUris { get; set; } = new string[0];
        [BindProperty(Name = OAuthClientParameters.RequireAuthTime)]
        [JsonPropertyName(OAuthClientParameters.RequireAuthTime)]
        public bool RequireAuthTime { get; set; } = false;
        [JsonPropertyName(OAuthClientParameters.AuthorizationSignedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationSignedResponseAlg { get; set; } = null;
        [JsonPropertyName(OAuthClientParameters.AuthorizationEncryptedResponseAlg)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationEncryptedResponseAlg { get; set; } = null;
        [JsonPropertyName(OAuthClientParameters.AuthorizationEncryptedResponseEnc)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationEncryptedResponseEnc { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.CredentialOfferEndpoint)]
        [JsonPropertyName(OAuthClientParameters.CredentialOfferEndpoint)]
        public string? CredentialOfferEndpoint { get; set; } = null;
        [JsonPropertyName(OAuthClientParameters.AuthorizationDataTypes)]
        public ICollection<string> AuthorizationDataTypes { get; set; } = new List<string>();
        [BindProperty(Name = OAuthClientParameters.DPOPBoundAccessTokens)]
        [JsonPropertyName(OAuthClientParameters.DPOPBoundAccessTokens)]
        public bool DPOPBoundAccessTokens { get; set; } = false;
        [JsonIgnore]
        public ICollection<TranslationRequest> Translations { get; set; } = new List<TranslationRequest>();

        public void Apply(Client client, IdServerHostOptions options)
        {
            client.RedirectionUrls = RedirectUris;
            client.SectorIdentifierUri = SectorIdentifierUri;
            client.DefaultAcrValues = DefaultAcrValues;
            client.PostLogoutRedirectUris = PostLogoutRedirectUris;
            client.RequireAuthTime = RequireAuthTime;
            if (GrantTypes == null || !GrantTypes.Any()) client.GrantTypes = new[] { AuthorizationCodeHandler.GRANT_TYPE };
            else client.GrantTypes = GrantTypes.ToList();

            if (string.IsNullOrWhiteSpace(TokenAuthMethod)) client.TokenEndPointAuthMethod = options.DefaultTokenEndPointAuthMethod;
            else client.TokenEndPointAuthMethod = TokenAuthMethod;

            if (ResponseTypes == null || !ResponseTypes.Any()) client.ResponseTypes = new[] { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE };
            else client.ResponseTypes = ResponseTypes;

            if (string.IsNullOrWhiteSpace(TokenSignedResponseAlg)) client.TokenSignedResponseAlg = options.DefaultTokenSignedResponseAlg;
            else client.TokenSignedResponseAlg = TokenSignedResponseAlg;

            if (string.IsNullOrWhiteSpace(TokenEncryptedResponseAlg)) client.TokenEncryptedResponseAlg = options.DefaultTokenEncrypteAlg;
            else client.TokenEncryptedResponseAlg= TokenEncryptedResponseAlg;

            if (string.IsNullOrWhiteSpace(TokenEncryptedResponseEnc)) client.TokenEncryptedResponseEnc = options.DefaultTokenEncryptedEnc;
            else client.TokenEncryptedResponseEnc = TokenEncryptedResponseEnc;

            if (string.IsNullOrWhiteSpace(ApplicationType)) client.ApplicationType = "web";
            else client.ApplicationType = ApplicationType;

            if (string.IsNullOrWhiteSpace(SubjectType)) client.SubjectType = options.DefaultSubjectType;
            else client.SubjectType = SubjectType;

            if (string.IsNullOrWhiteSpace(IdTokenSignedResponseAlg)) client.IdTokenSignedResponseAlg = SecurityAlgorithms.RsaSha256;
            else client.IdTokenSignedResponseAlg = IdTokenSignedResponseAlg;

            if (DefaultMaxAge == null) client.DefaultMaxAge = options.DefaultMaxAge;
            else client.DefaultMaxAge = DefaultMaxAge;

            client.IdTokenEncryptedResponseAlg = IdTokenEncryptedResponseAlg;
            client.IdTokenEncryptedResponseEnc = IdTokenEncryptedResponseEnc;
            if (!string.IsNullOrWhiteSpace(client.IdTokenEncryptedResponseAlg) && string.IsNullOrWhiteSpace(IdTokenEncryptedResponseEnc)) client.IdTokenEncryptedResponseEnc = SecurityAlgorithms.Aes128CbcHmacSha256;

            client.UserInfoSignedResponseAlg= UserInfoSignedResponseAlg;
            client.UserInfoEncryptedResponseAlg = UserInfoEncryptedResponseAlg;
            client.UserInfoEncryptedResponseEnc = UserInfoEncryptedResponseEnc;
            if (!string.IsNullOrWhiteSpace(client.UserInfoEncryptedResponseAlg) && string.IsNullOrWhiteSpace(UserInfoEncryptedResponseAlg)) client.UserInfoEncryptedResponseEnc = SecurityAlgorithms.Aes128CbcHmacSha256;

            client.RequestObjectSigningAlg = RequestObjectSigningAlg;
            client.RequestObjectEncryptionAlg = RequestObjectEncryptionAlg;
            client.RequestObjectEncryptionEnc = RequestObjectEncryptionEnc;
            if (!string.IsNullOrWhiteSpace(client.RequestObjectEncryptionAlg) && string.IsNullOrWhiteSpace(RequestObjectEncryptionEnc)) client.RequestObjectEncryptionEnc = SecurityAlgorithms.Aes128CbcHmacSha256;

            if (!string.IsNullOrWhiteSpace(RequestObjectEncryptionEnc)) client.RequestObjectEncryptionEnc = RequestObjectEncryptionEnc;

            client.BCTokenDeliveryMode = BCTokenDeliveryMode;
            client.BCClientNotificationEndpoint = BCClientNotificationEndpoint;
            client.BCAuthenticationRequestSigningAlg = BCAuthenticationRequestSigningAlg;
            client.BCUserCodeParameter = BCUserCodeParameter;

            client.FrontChannelLogoutUri = FrontChannelLogoutUri;
            client.FrontChannelLogoutSessionRequired = FrontChannelLogoutSessionRequired;

            client.BackChannelLogoutUri = BackChannelLogoutUri;
            client.BackChannelLogoutSessionRequired = BackChannelLogoutSessionRequired;

            client.InitiateLoginUri = InitiateLoginUri;

            if (string.IsNullOrWhiteSpace(AuthorizationSignedResponseAlg)) client.AuthorizationSignedResponseAlg = SecurityAlgorithms.RsaSha256;
            else client.AuthorizationSignedResponseAlg = AuthorizationSignedResponseAlg;
            client.AuthorizationEncryptedResponseAlg = AuthorizationEncryptedResponseAlg;
            client.AuthorizationEncryptedResponseEnc = AuthorizationEncryptedResponseEnc;

            if (AuthorizationDataTypes != null && AuthorizationDataTypes.Any())
                client.AuthorizationDataTypes = AuthorizationDataTypes;

            client.CredentialOfferEndpoint = CredentialOfferEndpoint;
            client.DPOPBoundAccessTokens = DPOPBoundAccessTokens;
        }
    }
}
