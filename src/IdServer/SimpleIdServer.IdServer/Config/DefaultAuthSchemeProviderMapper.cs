// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultAuthSchemeProviderMapper
{
    public static List<AuthenticationSchemeProviderMapper> All => new List<AuthenticationSchemeProviderMapper>
    {
        new AuthenticationSchemeProviderMapper
        {
            Id = Guid.NewGuid().ToString(),
            MapperType = MappingRuleTypes.SUBJECT,
            Name = "Subject",
            SourceClaimName = ClaimTypes.NameIdentifier
        },
        new AuthenticationSchemeProviderMapper
        {
            Id = Guid.NewGuid().ToString(),
            MapperType = MappingRuleTypes.USERPROPERTY,
            Name = "Firstname",
            SourceClaimName = ClaimTypes.Name,
            TargetUserProperty = nameof(User.Firstname)
        },
        new AuthenticationSchemeProviderMapper
        {
            Id = Guid.NewGuid().ToString(),
            MapperType = MappingRuleTypes.USERPROPERTY,
            Name = "Lastname",
            SourceClaimName = ClaimTypes.GivenName,
            TargetUserProperty = nameof(User.Lastname)
        },
        new AuthenticationSchemeProviderMapper
        {
            Id = Guid.NewGuid().ToString(),
            MapperType = MappingRuleTypes.USERPROPERTY,
            Name = "Email",
            SourceClaimName = ClaimTypes.Email,
            TargetUserProperty = nameof(User.Email)
        },
        new AuthenticationSchemeProviderMapper
        {
            Id = Guid.NewGuid().ToString(),
            MapperType = MappingRuleTypes.USERATTRIBUTE,
            Name = "Gender",
            SourceClaimName = ClaimTypes.Gender,
            TargetUserAttribute = JwtRegisteredClaimNames.Gender
        },
        new AuthenticationSchemeProviderMapper
        {
            Id = Guid.NewGuid().ToString(),
            MapperType = MappingRuleTypes.USERATTRIBUTE,
            Name = "DateOfBirth",
            SourceClaimName = ClaimTypes.DateOfBirth,
            TargetUserAttribute = JwtRegisteredClaimNames.Birthdate
        }
    };
}
