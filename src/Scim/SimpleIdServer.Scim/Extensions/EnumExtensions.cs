// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.ComponentModel;

namespace SimpleIdServer.Scim.Extensions
{
    public static class EnumExtensions
    {
        public static string ToName(this Enum value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length != 1)
            {
                return value.ToString();
            }

            var attribute = attributes[0] as DescriptionAttribute;
            return attribute.Description;
        }
    }
}
