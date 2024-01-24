// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.Api.Credential;
using SimpleIdServer.IdServer.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Parsers
{
    public interface ICredentialRequestParser
    {
        string Format { get; }
        Task<ExtractionResult> Extract(CredentialRequest request, CancellationToken cancellationToken);
    }

    public class ExtractionResult
    {
        public ExtractionResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ExtractionResult(ICredentialRequest request, CredentialTemplate credentialTemplate)
        {
            Request = request;
            CredentialTemplate = credentialTemplate;
        }

        public static ExtractionResult Error(string errorMessage) => new ExtractionResult(errorMessage);

        public static ExtractionResult Ok(ICredentialRequest request, CredentialTemplate credentialTemplate) => new ExtractionResult(request, credentialTemplate);

        public string ErrorMessage { get; private set; }
        public ICredentialRequest Request { get; private set; }
        public CredentialTemplate CredentialTemplate { get; private set; }
    }
}
