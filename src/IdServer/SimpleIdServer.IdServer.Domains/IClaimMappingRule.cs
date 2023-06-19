// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public interface IClaimMappingRule
    {
        MappingRuleTypes MapperType { get; set; }
        string? SourceUserAttribute { get; set; }
        string? SourceUserProperty { get; set; }
        string TargetClaimPath { get; set; }
        bool IsMultiValued { get; set; }
        TokenClaimJsonTypes? TokenClaimJsonType { get; set; }
    }

    public enum TokenClaimJsonTypes
    {
        STRING = 0,
        LONG = 1,
        INT = 2,
        BOOLEAN = 3,
        JSON = 4,
        DATETIME = 5
    }
}
