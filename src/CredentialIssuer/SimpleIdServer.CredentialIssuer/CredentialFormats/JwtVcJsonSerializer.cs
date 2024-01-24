// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.CredentialIssuer.CredentialFormats;

public class JwtVcJsonSerializer : ICredentialSerializer
{
    public string Format => "jwt_vc_json";
}