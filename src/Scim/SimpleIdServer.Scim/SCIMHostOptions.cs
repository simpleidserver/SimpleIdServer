// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim
{
    public class SCIMHostOptions
    {
        public SCIMHostOptions()
        {
            AuthenticationScheme = SCIMConstants.AuthenticationScheme;
            SCIMIdClaimName = "scim_id";
            MaxOperations = 1000;
            MaxPayloadSize = 1048576;
            MaxResults = 200;
            IgnoreUnsupportedCanonicalValues = true;
        }

        /// <summary>
        /// Authentication scheme.
        /// </summary>
        public string AuthenticationScheme { get; set; }
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
        /// <summary>
        /// Ignore unsupported canonical values. 
        /// If set to 'false' and the canonical value is not supported then an exception is thrown.
        /// </summary>
        public bool IgnoreUnsupportedCanonicalValues { get; set; }
    }
}
