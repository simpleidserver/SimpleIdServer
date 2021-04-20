// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OpenID.Domains
{
    public class BCAuthorizePermission : ICloneable
    {
        private BCAuthorizePermission()
        {

        }

        public string ConsentId { get; set; }
        public string Type { get; set; }
        public string PermissionId { get; set; }
        public string DisplayName { get; set; }
        public bool IsConfirmed { get; set; }

        public static BCAuthorizePermission Create(string consentId, string type, string permissionId, string displayName)
        {
            return new BCAuthorizePermission { ConsentId = consentId, Type = type, PermissionId = permissionId, DisplayName = displayName };
        }

        public void Confirm()
        {
            IsConfirmed = true;
        }

        public object Clone()
        {
            return new BCAuthorizePermission
            {
                 ConsentId = ConsentId,
                 Type = Type,
                 DisplayName = DisplayName,
                 PermissionId = PermissionId,
                 IsConfirmed = IsConfirmed
            };
        }
    }
}
