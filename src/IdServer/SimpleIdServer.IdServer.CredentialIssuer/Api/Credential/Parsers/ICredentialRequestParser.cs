// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Parsers
{
    public interface ICredentialRequestParser
    {
        string Format { get; }
        ExtractionResult Extract(CredentialRequest request);
    }

    public class ExtractionResult
    {
        public static ExtractionResult Error(string errorMessage) => new ExtractionResult();

        public static ExtractionResult Ok() => new ExtractionResult();
    }
}
