// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Seeding
{
    /// <summary>
    /// Represents a scope to seed.
    /// </summary>
    public class ScopeSeedDto
    {
        /// <summary>
        /// The scope's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The scope's type. Possible Values are IDENTITY, APIRESOURCE and ROLE
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The scope's protocol. Possible Values are OPENID, SAML and OAUTH.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// The scope's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates if this scope should appears in the Discovery endpoint.
        /// </summary>
        public bool IsExposedInConfigurationEdp { get; set; }

        /// <summary>
        /// The scope's component.
        /// </summary>
        public string Component { set; get; }

        /// <summary>
        /// The scope's component action. Possible Values are Manage and View.
        /// </summary>
        public string ComponentAction { get; set; }

        /// <summary>
        /// Realm to relate.
        /// </summary>
        public string Realm { get; set; }
    }
}
