// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Options
{
    /// <summary>
    /// Options for seeding via JSON file.
    /// </summary>
    public class JsonSeedingOptions
    {
        /// <summary>
        /// Indicates if seeding from JSON file is needed.
        /// </summary>
        public bool SeedFromJson { get; set; }

        /// <summary>
        /// Indicates the JSON file path.
        /// </summary>
        public string JsonFilePath { get; set; }
    }
}
