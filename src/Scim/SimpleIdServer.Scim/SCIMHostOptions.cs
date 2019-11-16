// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim
{
    public class SCIMHostOptions
    {
        public SCIMHostOptions()
        {
            AuthenticationScheme = SCIMConstants.AuthenticationScheme;
            UserSchemas = new List<SCIMSchema>
            {
                SCIMConstants.StandardSchemas.UserSchema,
                SCIMConstants.StandardSchemas.CommonSchema
            };
            GroupSchemas = new List<SCIMSchema>
            {
                SCIMConstants.StandardSchemas.GroupSchema,
                SCIMConstants.StandardSchemas.CommonSchema
            };
            SCIMIdClaimName = "scim_id";
            MaxOperations = 1000;
            MaxPayloadSize = 1048576;
            MaxResults = 200;
        }

        /// <summary>
        /// Authentication scheme.
        /// </summary>
        public string AuthenticationScheme { get; set; }
        /// <summary>
        /// User schemas URLS.
        /// </summary>
        public ICollection<SCIMSchema> UserSchemas { get; set; }
        /// <summary>
        /// Group schemas URLS.
        /// </summary>
        public ICollection<SCIMSchema> GroupSchemas { get; set; }
        /// <summary>
        /// Name of the claim used to get the scim identifier.
        /// </summary>
        public string SCIMIdClaimName { get; set; }
        /// <summary>
        /// An integer value specifying the maximum number of operations.
        /// </summary>
        public int MaxOperations { get; set; }
        /// <summary>
        /// An integer value specifying the maximum payload size in bytes.
        /// </summary>
        public int MaxPayloadSize { get; set; }
        /// <summary>
        /// An integer value specifying the maximum number of resources returned in a response.
        /// </summary>
        public int MaxResults { get; set; }
    }
}
