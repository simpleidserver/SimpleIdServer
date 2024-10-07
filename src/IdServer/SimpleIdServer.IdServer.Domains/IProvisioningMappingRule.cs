// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public interface IProvisioningMappingRule
    {
        public MappingRuleTypes MapperType { get; set; }
        public string? TargetUserAttribute { get; set; }
        public string? TargetUserProperty { get; set; }

        public static bool IsUnique(MappingRuleTypes mapperType)
        {
            return mapperType == MappingRuleTypes.SUBJECT || 
                mapperType == MappingRuleTypes.IDENTIFIER  || 
                mapperType == MappingRuleTypes.GROUPNAME;
        }
    }

    public enum MappingRuleTypes
    {
        USERATTRIBUTE = 0,
        USERPROPERTY = 1,
        SUBJECT = 2,
        IDENTIFIER = 3,
        GROUPNAME = 4,
        SCIM = 5
    }
}