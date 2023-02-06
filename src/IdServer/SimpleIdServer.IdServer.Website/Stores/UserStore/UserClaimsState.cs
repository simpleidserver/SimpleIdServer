// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    [FeatureState]
    public record UserClaimsState
    {
        public UserClaimsState() { }

        public UserClaimsState(bool isLoading, IEnumerable<UserClaim> userClaims)
        {
            UserClaims = userClaims.Select(c => new SelectableUserClaim(c));
            Count = userClaims.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableUserClaim>? UserClaims { get; set; } = new List<SelectableUserClaim>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
        public bool HasLeastOneUserIsSelected
        {
            get
            {
                return UserClaims == null ? false : UserClaims.Any(c => c.IsSelected);
            }
        }
    }

    public class SelectableUserClaim
    {
        public SelectableUserClaim(UserClaim user)
        {
            Value = user;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public UserClaim Value { get; set; }
    }
}
