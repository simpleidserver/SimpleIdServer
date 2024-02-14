// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Authenticate.AssertionParsers
{
    public interface IClientAssertionParser
    {
        string Type { get; }
        bool TryExtractClientId(string value, out string clientId);
        ClientAssertionResult Parse(string value);
    }

    public class ClientAssertionResult
    {
        public ClientAssertionStatus Status { get; set; } = ClientAssertionStatus.OK;
        public JsonWebToken JsonWebToken { get; set; }
        public string ErrorMessage { get; set; }

        public static ClientAssertionResult Invalid(string errorMessage) => new ClientAssertionResult { Status = ClientAssertionStatus.ERROR, ErrorMessage = errorMessage };

        public static ClientAssertionResult Ok(JsonWebToken jsonWebToken) => new ClientAssertionResult { JsonWebToken = jsonWebToken };
    }

    public enum ClientAssertionStatus
    {
        ERROR = 0,
        OK = 1
    }

    public class ClientJwtAssertionParser : IClientAssertionParser
    {
        public const string TYPE = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";

        public string Type => TYPE;

        public bool TryExtractClientId(string value, out string clientId)
        {
            clientId = null;
            var result = Parse(value);
            if (result.Status == ClientAssertionStatus.ERROR) throw new OAuthException(ErrorCodes.INVALID_REQUEST, result.ErrorMessage);
            if (result.JsonWebToken.IsEncrypted) return false;
            var payload = result.JsonWebToken.GetClaimJson();
            clientId = payload.GetStr(OpenIdConnectParameterNames.Iss);
            return true;
        }

        public ClientAssertionResult Parse(string value)
        {
            var handler = new JsonWebTokenHandler();
            var canRead = handler.CanReadToken(value);
            if (!canRead) return ClientAssertionResult.Invalid(Global.BadClientAssertionJwt);
            var token = handler.ReadJsonWebToken(value);
            return ClientAssertionResult.Ok(token);
        }
    }
}
