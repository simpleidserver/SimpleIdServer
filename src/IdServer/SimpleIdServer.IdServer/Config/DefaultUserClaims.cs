// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using System.Collections.Generic;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultUserClaims
{
    public static List<string> All => new List<string>
    {
        JwtRegisteredClaimNames.Sub,
        JwtRegisteredClaimNames.Name,
        JwtRegisteredClaimNames.GivenName,
        JwtRegisteredClaimNames.FamilyName,
        MiddleName,
        NickName,
        PreferredUserName,
        Profile,
        Picture,
        JwtRegisteredClaimNames.Website,
        JwtRegisteredClaimNames.Email,
        EmailVerified,
        JwtRegisteredClaimNames.Gender,
        BirthDate,
        ZoneInfo,
        Locale,
        JwtRegisteredClaimNames.PhoneNumber,
        JwtRegisteredClaimNames.PhoneNumberVerified,
        Address,
        UpdatedAt,
        Role,
        ScimId,
        ScimLocation

    };

    public static string MiddleName = "middle_name";
    public static string NickName = "nickname";
    public static string PreferredUserName = "preferred_username";
    public static string Profile = "profile";
    public static string Picture = "picture";
    public static string BirthDate = "birthdate";
    public static string ZoneInfo = "zoneinfo";
    public static string Locale = "locale";
    public static string UpdatedAt = "updated_at";
    public static string EmailVerified = "email_verified";
    public static string Address = "address";
    public static string Role = "role";
    public static string ScimId = "scim_id";
    public static string ScimLocation = "scim_location";
    public static string Events = "events";
    public static string Amrs = "amrs";
}
