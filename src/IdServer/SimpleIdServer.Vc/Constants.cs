// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.Vc
{
    public static class Constants
    {
        public const string DefaultVerifiableCredentialContext = "https://www.w3.org/2018/credentials/v1";
        public const string DefaultVerifiableCredentialType = "VerifiableCredential";
        public const string DefaultVerifiablePresentationType = "VerifiablePresentation";

        public static class CredentialTemplateProfiles
        {
            public const string W3CVerifiableCredentials = "jwt_vc_json";
        }

        public static class CredentialSubjectDisplayTypes
        {
            public const string String = "string";
            public const string Number = "number";
            public const string Jpeg = "image/jpeg";
        }

        public static ICollection<string> AllCredentialSubjectDisplayTypes = new string[]
        {
            CredentialSubjectDisplayTypes.String,
            CredentialSubjectDisplayTypes.Number,
            CredentialSubjectDisplayTypes.Jpeg
        };

        public static IEnumerable<string> AllCredentialTemplateProfiles = new string[]
        {
            CredentialTemplateProfiles.W3CVerifiableCredentials
        };
    }
}
