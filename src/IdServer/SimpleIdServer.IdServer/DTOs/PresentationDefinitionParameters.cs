// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs
{
    public static class PresentationDefinitionParameters
    {
        public const string Id = "id";
        public const string InputDescriptors = "input_descriptors";
        public const string Name = "name";
        public const string Purpose = "purpose";
        public const string Format = "format";
        public const string Constraints = "constraints";
        public const string Alg = "alg";
        public const string ProofType = "proof_type";
        public const string Fields = "fields";
        public const string LimitDisclosure = "limit_disclosure";
        public const string Path = "path";
        public const string Filter = "filter";
        public const string Optional = "optional";
        public const string Required = "required";
    }

    public static class LimitDisclosures
    {
        public const string Required = "required";
        public const string Preferred = "preferred";
    }
}
