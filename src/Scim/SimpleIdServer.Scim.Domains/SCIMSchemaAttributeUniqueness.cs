// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.ComponentModel;

namespace SimpleIdServer.Scim.Domains
{
    public enum SCIMSchemaAttributeUniqueness
    {
        /// <summary>
        /// The values are not intended to be unique in any way.
        /// </summary>
        [Description("none")]
        NONE = 0,
        /// <summary>
        /// The value SHOULD be unique within the context of the current SCIM endpoint (or tenancy) and MAY be globally unique (e.g., a "username", email address, or other server-generated key or counter).
        /// </summary>
        [Description("server")]
        SERVER = 1,
        /// <summary>
        /// The value SHOULD be globally unique (e.g., an email address, a GUID, or other value).  No two resources on any server SHOULD possess the same value.
        /// </summary>
        [Description("global")]
        GLOBAL = 2
    }
}
