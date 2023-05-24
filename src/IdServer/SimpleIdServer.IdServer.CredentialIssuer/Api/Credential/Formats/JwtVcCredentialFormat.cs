// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.Vc.Jwt;
using System.Text.Json.Nodes;
using static SimpleIdServer.Vc.Constants;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Formats
{
    public class JwtVcCredentialFormat : BaseVcCredentialFormat, ICredentialFormat
    {
        public string Format => CredentialTemplateProfiles.W3CVerifiableCredentials;

        public JsonObject Transform(CredentialFormatParameter parameter)
        {
            var vc = BuildVC(parameter);
            var privateKey = parameter.User.DidPrivateHex;
            var jwt = VcJwtBuilder.GenerateToken(parameter.IdentityDocument, vc, privateKey.HexToByteArray());
            return new JsonObject
            {
                { CredentialResultNames.CNonce, parameter.CNonce },
                { CredentialResultNames.CNonceExpiresIn, parameter.CNonceExpiresIn },
                { CredentialResultNames.Credential, jwt },
                { CredentialResultNames.Format, "ldp_vc" }
            };
        }
    }
}
