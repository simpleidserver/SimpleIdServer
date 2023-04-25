// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class VerifiableCredentialContext
    {
        public string Id { get; set; } = null!;
        public string Value { get; set; } = null!;
        public VerifiableCredentialContextSources Source { get; set; } = VerifiableCredentialContextSources.DB;
    }

    public enum VerifiableCredentialContextSources
    {
        URL = 0,
        DB = 1
    }
}
