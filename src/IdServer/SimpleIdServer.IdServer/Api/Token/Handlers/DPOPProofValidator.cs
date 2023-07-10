// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public interface IDPOPProofValidator
    {
        void Validate(HandlerContext context);
    }

    public class DPOPProofValidator : IDPOPProofValidator
    {
        private readonly IdServerHostOptions _options;

        public DPOPProofValidator(IOptions<IdServerHostOptions> options)
        {
            _options = options.Value;
        }

        public void Validate(HandlerContext context)
        {
            if (!context.Client.DPOPBoundAccessTokens) return;
            if (!context.Request.HttpHeader.ContainsKey(Constants.DPOPHeaderName)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_DPOP_PROOF);
            var value = context.Request.HttpHeader[Constants.DPOPHeaderName];
            string dpopProof = null;
            if (value is JsonArray)
            {
                var values = value as JsonArray;
                if(values.Count > 1) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.TOO_MANY_DPOP_HEADER);
                dpopProof = values.First().AsValue().GetValue<string>();
            }
            else
            {
                if (!(value is JsonValue)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_DPOP_HEADER);
                dpopProof = (context.Request.HttpHeader[Constants.DPOPHeaderName] as JsonValue).GetValue<string>();
            }

            var handler = new DPoPHandler();
            var validationResult = handler.Validate(dpopProof, Constants.AllSigningAlgs, context.Request.HttpMethod, $"{context.GetIssuer()}/{Constants.EndPoints.Token}", _options.DPoPLifetimeSeconds);
            if (!validationResult.IsValid) throw new OAuthException(ErrorCodes.INVALID_DPOP_PROOF, validationResult.ErrorMessage);
        }
    }
}
