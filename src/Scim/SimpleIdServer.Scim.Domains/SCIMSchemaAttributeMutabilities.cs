// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.ComponentModel;

namespace SimpleIdServer.Scim.Domains
{
    public enum SCIMSchemaAttributeMutabilities
    {
        /// <summary>
        /// The attribute SHALL NOT be modified.
        /// </summary>
        [Description("readOnly")]
        READONLY = 0,
        /// <summary>
        /// The attribute MAY be updated and read at any time. This is the default value.
        /// </summary>
        [Description("readWrite")]
        READWRITE = 1,
        /// <summary>
        /// The attribute MAY be defined at resource creation (e.g., POST) or at record replacement via a request (e.g., a PUT).  The attribute SHALL NOT be updated.
        /// </summary>
        [Description("immutable")]
        IMMUTABLE = 2,
        /// <summary>
        /// The attribute MAY be updated at any time.  Attribute values SHALL NOT be returned (e.g., because the value is a stored hash).  Note: An attribute with a mutability of "writeOnly" usually also has a returned setting of "never".
        /// </summary>
        [Description("writeOnly")]
        WRITEONLY = 3
    }
}