// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.ComponentModel;

namespace SimpleIdServer.Scim.Domains
{
    public enum SCIMSchemaAttributeReturned
    {
        /// <summary>
        /// The attribute is always returned, regardless of the contents of the "attributes" parameter. 
        /// </summary>
        [Description("always")]
        ALWAYS = 0,
        /// <summary>
        /// The attribute is never returned.  This may occur because the original attribute value (e.g., a hashed value) is not retained by the service provider.
        /// </summary>
        [Description("never")]
        NEVER = 1,
        /// <summary>
        /// The attribute is returned by default in all SCIM operation responses where attribute values are returned.
        /// </summary>
        [Description("default")]
        DEFAULT = 2,
        /// <summary>
        /// The attribute is returned in response to any PUT, POST, or PATCH operations if the attribute was specified by the client.
        /// </summary>
        [Description("request")]
        REQUEST = 3
    }
}