// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs.Seeds;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Services.Seeding.Interfaces
{
    /// <summary>
    /// Defines the methods to seed from an external resource.
    /// </summary>
    internal interface ISeedingService
    {
        /// <summary>
        /// Allows to get all the entities found in the resource.
        /// </summary>
        /// <returns>Entities found.</returns>
        Task<SeedsDto?> GetDataFromResourceAsync();

        /// <summary>
        /// Allows to seed all the entities found in the resource.
        /// </summary>
        /// <returns></returns>
        Task SeedDataAsync();

        /// <summary>
        /// Allows to seed all the entities found in the resource.
        /// </summary>
        /// <param name="seeds">The entities to seed.</param>
        /// <returns></returns>
        Task SeedDataAsync(SeedsDto seeds);
    }
}
