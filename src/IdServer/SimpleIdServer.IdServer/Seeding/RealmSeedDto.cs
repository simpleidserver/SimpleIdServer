// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Seeding
{
    /// <summary>
    /// Represents a realm to seed.
    /// </summary>
    public class RealmSeedDto
    {
        /// <summary>
        /// The realm's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The realm's description.
        /// </summary>
        public string Description { get; set; }
    }
}
