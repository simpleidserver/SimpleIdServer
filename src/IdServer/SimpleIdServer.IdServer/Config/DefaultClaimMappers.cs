// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Config;

public class DefaultClaimMappers
{
    public static ScopeClaimMapper Subject = ScopeClaimMapper.CreateOpenIdSubjectClaim();
    public static ScopeClaimMapper Name = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.Name, JwtRegisteredClaimNames.Name, nameof(User.Firstname));
    public static ScopeClaimMapper FamilyName = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.FamilyName, JwtRegisteredClaimNames.FamilyName, nameof(User.Lastname));
    public static ScopeClaimMapper UpdatedAt = ScopeClaimMapper.CreateOpenIdPropertyClaim(Config.DefaultUserClaims.UpdatedAt, Config.DefaultUserClaims.UpdatedAt, nameof(User.UpdateDateTime), TokenClaimJsonTypes.DATETIME);
    public static ScopeClaimMapper Email = ScopeClaimMapper.CreateOpenIdPropertyClaim(JwtRegisteredClaimNames.Email, JwtRegisteredClaimNames.Email, nameof(User.Email));
    public static ScopeClaimMapper EmailVerified = ScopeClaimMapper.CreateOpenIdPropertyClaim(Config.DefaultUserClaims.EmailVerified, Config.DefaultUserClaims.EmailVerified, nameof(User.EmailVerified), TokenClaimJsonTypes.BOOLEAN);
    public static ScopeClaimMapper UniqueName = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.UniqueName, JwtRegisteredClaimNames.UniqueName, JwtRegisteredClaimNames.UniqueName);
    public static ScopeClaimMapper GivenName = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.GivenName, JwtRegisteredClaimNames.GivenName, JwtRegisteredClaimNames.GivenName);
    public static ScopeClaimMapper MiddleName = ScopeClaimMapper.CreateOpenIdAttributeClaim(Config.DefaultUserClaims.MiddleName, Config.DefaultUserClaims.MiddleName, Config.DefaultUserClaims.MiddleName);
    public static ScopeClaimMapper NickName = ScopeClaimMapper.CreateOpenIdAttributeClaim(Config.DefaultUserClaims.NickName, Config.DefaultUserClaims.NickName, Config.DefaultUserClaims.NickName);
    public static ScopeClaimMapper PreferredUserName = ScopeClaimMapper.CreateOpenIdAttributeClaim(Config.DefaultUserClaims.PreferredUserName, Config.DefaultUserClaims.PreferredUserName, Config.DefaultUserClaims.PreferredUserName);
    public static ScopeClaimMapper Profile = ScopeClaimMapper.CreateOpenIdAttributeClaim(Config.DefaultUserClaims.Profile, Config.DefaultUserClaims.Profile, Config.DefaultUserClaims.Profile);
    public static ScopeClaimMapper Picture = ScopeClaimMapper.CreateOpenIdAttributeClaim(Config.DefaultUserClaims.Picture, Config.DefaultUserClaims.Picture, Config.DefaultUserClaims.Picture);
    public static ScopeClaimMapper WebSite = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Website, JwtRegisteredClaimNames.Website, JwtRegisteredClaimNames.Website);
    public static ScopeClaimMapper Gender = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Gender, JwtRegisteredClaimNames.Gender, JwtRegisteredClaimNames.Gender);
    public static ScopeClaimMapper BirthDate = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.Birthdate, JwtRegisteredClaimNames.Birthdate, JwtRegisteredClaimNames.Birthdate);
    public static ScopeClaimMapper ZoneInfo = ScopeClaimMapper.CreateOpenIdAttributeClaim(Config.DefaultUserClaims.ZoneInfo, Config.DefaultUserClaims.ZoneInfo, Config.DefaultUserClaims.ZoneInfo);
    public static ScopeClaimMapper Locale = ScopeClaimMapper.CreateOpenIdAttributeClaim(Config.DefaultUserClaims.Locale, Config.DefaultUserClaims.Locale, Config.DefaultUserClaims.Locale);
    public static List<ScopeClaimMapper> Address = ScopeClaimMapper.CreateOpenIdAddressClaim();
    public static ScopeClaimMapper PhoneNumber = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.PhoneNumber, JwtRegisteredClaimNames.PhoneNumber, JwtRegisteredClaimNames.PhoneNumber);
    public static ScopeClaimMapper PhoneNumberVerified = ScopeClaimMapper.CreateOpenIdAttributeClaim(JwtRegisteredClaimNames.PhoneNumberVerified, JwtRegisteredClaimNames.PhoneNumberVerified, JwtRegisteredClaimNames.PhoneNumberVerified, TokenClaimJsonTypes.BOOLEAN);
    public static ScopeClaimMapper Role = ScopeClaimMapper.CreateOpenIdAttributeClaimArray(Config.DefaultUserClaims.Role, Config.DefaultUserClaims.Role, Config.DefaultUserClaims.Role);
    public static ScopeClaimMapper ScimId = ScopeClaimMapper.CreateOpenIdAttributeClaim("scim_id", "scim_id", "scim_id");
    public static ScopeClaimMapper SAMLNameIdentifier = ScopeClaimMapper.CreateSAMLNameIdentifierClaim();
    public static ScopeClaimMapper SAMLName = ScopeClaimMapper.CreateSAMLPropertyClaim("name", ClaimTypes.Name, nameof(User.Firstname));
}
