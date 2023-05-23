// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class CredentialTemplateClaimMapper : IClaimMappingRule
    {
        public string Id { get; set; } = null!;
        public MappingRuleTypes MapperType { get; set; }
        public string? SourceUserAttribute { get; set; } = null;
        public string? SourceUserProperty { get; set; } = null;
        public string TargetClaimPath { get; set; } = null!;
        public bool IsMultiValued { get; set; }
        public TokenClaimJsonTypes? TokenClaimJsonType { get; set; }
        public string CredentialTemplateId { get; set; } = null!;
        public CredentialTemplate CredentialTemplate { get; set; } = null!;
    }
}
