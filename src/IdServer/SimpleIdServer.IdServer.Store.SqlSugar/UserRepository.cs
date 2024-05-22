// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    internal class UserRepository : IUserRepository
    {
        public void Add(User user)
        {
            throw new NotImplementedException();
        }

        public Task BulkUpdate(List<UserClaim> userClaims)
        {
            throw new NotImplementedException();
        }

        public Task BulkUpdate(List<User> users)
        {
            throw new NotImplementedException();
        }

        public Task BulkUpdate(List<RealmUser> userRealms)
        {
            throw new NotImplementedException();
        }

        public Task BulkUpdate(List<GroupUser> groupUsers)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByClaim(string name, string value, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByEmail(string email, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByExternalAuthProvider(string scheme, string sub, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetById(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetById(string id, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetBySubject(string subject, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsersById(IEnumerable<string> ids, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsersBySubjects(IEnumerable<string> subjects, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsClaimExists(string name, string value, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEmailExists(string email, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExternalAuthProviderExists(string scheme, string sub, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSubjectExists(string sub, string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> NbUsers(string realm, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Remove(IEnumerable<User> users)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SearchResult<User>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
