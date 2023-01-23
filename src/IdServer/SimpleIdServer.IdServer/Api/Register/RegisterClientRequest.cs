// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;

namespace SimpleIdServer.IdServer.Api.Register
{
    [JsonConverter(typeof(RegisterClientRequestConverter))]
    public class RegisterClientRequest
    {
        [BindProperty(Name = OAuthClientParameters.ApplicationType)]
        public string? ApplicationType { get; set; }
        [BindProperty(Name = OAuthClientParameters.SoftwareStatement)]
        [JsonPropertyName(OAuthClientParameters.SoftwareStatement)]
        public string? SoftwareStatement { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.ClientName)]
        [JsonPropertyName(OAuthClientParameters.ClientName)]
        public string? ClientName { get; set; } = null;
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
        public IEnumerable<string> RedirectUris { get; set; } = new List<string>();
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
        [JsonIgnore]
        public ICollection<RegisterTranslation> Translations = new List<RegisterTranslation>();

        public void Apply(Client client, IdServerHostOptions options)
        {
            var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (!string.IsNullOrWhiteSpace(ClientName)) client.AddClientName(language, ClientName);
            if (GrantTypes == null || !GrantTypes.Any()) client.GrantTypes = new[] { AuthorizationCodeHandler.GRANT_TYPE };
            else client.GrantTypes = GrantTypes.ToList();
            if (string.IsNullOrWhiteSpace(TokenAuthMethod)) client.TokenEndPointAuthMethod = options.DefaultTokenEndPointAuthMethod;
            else client.TokenEndPointAuthMethod = TokenAuthMethod;
            if (ResponseTypes == null || !ResponseTypes.Any()) client.ResponseTypes = new[] { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE };
            else client.ResponseTypes = ResponseTypes;
            if (string.IsNullOrWhiteSpace(TokenSignedResponseAlg)) client.TokenSignedResponseAlg = options.DefaultTokenSignedResponseAlg;
            else client.TokenSignedResponseAlg = TokenSignedResponseAlg;
            if (string.IsNullOrWhiteSpace(TokenEncryptedResponseAlg)) client.TokenEncryptedResponseAlg = options.DefaultTokenEncrypteAlg;
            if (string.IsNullOrWhiteSpace(TokenEncryptedResponseEnc)) client.TokenEncryptedResponseEnc = options.DefaultTokenEncryptedEnc;
            else client.TokenEncryptedResponseEnc = TokenEncryptedResponseEnc;
        }
    }

    public class RegisterTranslation
    {
        public string Language { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
