// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public interface IProvisioningMappingRule
    {
        public MappingRuleTypes MapperType { get; set; }
        public string? TargetUserAttribute { get; set; }
        public string? TargetUserProperty { get; set; }
    }

    public enum MappingRuleTypes
    {
        USERATTRIBUTE = 0,
        USERPROPERTY = 1,
        SUBJECT = 2
    }
}
