// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Resources;
using System;
using System.Net;

namespace SimpleIdServer.Saml.Validators
{
    public static class SAMLValidator
    {
        public static T CheckSaml<T>(string saml, string relayState, bool isRequester = true) where T : class
        {
            string errorCode = isRequester ? Saml.Constants.StatusCodes.Requester : Saml.Constants.StatusCodes.Responder;
            if (string.IsNullOrWhiteSpace(saml))
            {
                throw new SamlException(HttpStatusCode.BadRequest, errorCode, string.Format(Global.MissingParameter, nameof(saml)));
            }

            if (string.IsNullOrWhiteSpace(relayState))
            {
                throw new SamlException(HttpStatusCode.BadRequest, errorCode, string.Format(Global.MissingParameter, nameof(relayState)));
            }

            string decompressed;
            try
            {
                decompressed = Compression.Decompress(saml);
            }
            catch (Exception ex)
            {
                throw new SamlException(HttpStatusCode.BadRequest, errorCode, Global.BadSamlDecompression);
            }

            T request = null;
            try
            {
                request = decompressed.DeserializeXml<T>();
            }
            catch (Exception ex)
            {
                throw new SamlException(HttpStatusCode.BadRequest, errorCode, Global.BadSamlDeserialization);
            }

            return request;
        }
    }
}
