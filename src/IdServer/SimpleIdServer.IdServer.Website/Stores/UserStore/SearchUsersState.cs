// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    [FeatureState]
    public record SearchUsersState
    {       
        public SearchUsersState() { }

        public SearchUsersState(bool isLoading, IEnumerable<User> users)
        {
            Users = users.Select(c => new SelectableUser(c));
            Count = users.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableUser>? Users { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
        public bool HasLeastOneUserIsSelected
        {
            get
            {
                return Users == null ? false : Users.Any(c => c.IsSelected);
            }
        }
    }

    public class SelectableUser
    {
        public SelectableUser(User user)
        {
            Value = user;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public User Value { get; set; }
    }
}
