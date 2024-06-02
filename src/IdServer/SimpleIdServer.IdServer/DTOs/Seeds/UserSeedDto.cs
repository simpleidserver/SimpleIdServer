// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.DTOs.Seeds
{
    /// <summary>
    /// Represents an user to seed.
    /// </summary>
    public class UserSeedDto
    {
        /// <summary>
        /// The user's name for login.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// (Optional) The user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// (Optional) The user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// (Optional) The user's email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The user's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// (Optional) Array of user roles.
        /// </summary>
        public string[] Roles { get; set; } = [];
    }
}
