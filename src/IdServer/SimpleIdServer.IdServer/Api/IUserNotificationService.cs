// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api
{
    public interface IUserNotificationService
    {
        string Name { get; }
        Task Send(string message, User user, ICollection<UserClaim> userClaims);
        Task Send(string message, string destination);
    }
}
