// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Builders
{
    public class CredentialOfferBuilder
    {
        private readonly UserCredentialOffer _credentialOffer;

        private CredentialOfferBuilder(UserCredentialOffer credentialOffer)
        {
            _credentialOffer = credentialOffer;
        }

        public static CredentialOfferBuilder New(string id, IEnumerable<string> credentialNames, User user)
        {
            return new CredentialOfferBuilder(new UserCredentialOffer
            {
                Id = id,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                UserId = user.Id,
                User = user,
                CredentialNames = credentialNames.ToList()
            });
        }

        public static CredentialOfferBuilder New(IEnumerable<string> credentialNames, User user) => New(Guid.NewGuid().ToString(), credentialNames, user);

        public UserCredentialOffer Build() => _credentialOffer;
    }
}
