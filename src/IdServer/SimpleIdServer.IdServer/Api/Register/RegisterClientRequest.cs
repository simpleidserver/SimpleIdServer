// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;

namespace SimpleIdServer.IdServer.Api.Register
{
    public class RegisterClientRequest
    {
        [BindProperty(Name = OAuthClientParameters.ClientId)]
        [JsonPropertyName(OAuthClientParameters.ClientId)]
        public string ClientId { get; set; }
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
        public IEnumerable<string> ResponseTypes { get; set; } = new string[0];
        [BindProperty(Name = OAuthClientParameters.TokenSignedResponseAlg)]
        [JsonPropertyName(OAuthClientParameters.TokenSignedResponseAlg)]
        public string? TokenSignedResponseAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.TokenEncryptedResponseAlg)]
        [JsonPropertyName(OAuthClientParameters.TokenEncryptedResponseAlg)]
        public string? TokenEncryptedResponseAlg { get; set; } = null;
        [BindProperty(Name = OAuthClientParameters.TokenEncryptedResponseEnc)]
        [JsonPropertyName(OAuthClientParameters.TokenEncryptedResponseEnc)]
        public string? TokenEncryptedResponseEnc { get; set; } = null;

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
}
