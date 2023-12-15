// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did
{
    public interface IDIDGenerator
    {
        string Method { get; }
        Task<DIDGenerationResult> Generate(Dictionary<string, string> parameters, CancellationToken cancellationToken);
    }

    public record DIDGenerationResult
    {
        private DIDGenerationResult(string did, string privateKey)
        {
            DID = did;
            PrivateKey = privateKey;
        }

        private DIDGenerationResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string DID { get; private set; }
        public string PrivateKey { get; private set; }
        public string ErrorMessage { get; private set; }

        public static DIDGenerationResult Ok(string did, string privateKey) => new DIDGenerationResult(did, privateKey);

        public static DIDGenerationResult Nok(string errorMessage) => new DIDGenerationResult(errorMessage);
    }
}
