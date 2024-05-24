// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IUserRepository
{
    Task<User> GetBySubject(string subject, string realm, CancellationToken cancellationToken);
    Task<User> GetById(string id, CancellationToken cancellationToken);
    Task<User> GetById(string id, string realm, CancellationToken cancellationToken);
    Task<User> GetByEmail(string email, string realm, CancellationToken cancellationToken);
    Task<User> GetByExternalAuthProvider(string scheme, string sub, string realm, CancellationToken cancellationToken);
    Task<User> GetByClaim(string name, string value, string realm, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetUsersById(IEnumerable<string> ids, string realm, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetUsersBySubjects(IEnumerable<string> subjects, string realm, CancellationToken cancellationToken);
    Task<int> NbUsers(string realm, CancellationToken cancellationToken);
    Task<bool> IsExternalAuthProviderExists(string scheme, string sub, string realm, CancellationToken cancellationToken);
    Task<bool> IsSubjectExists(string sub, string realm, CancellationToken cancellationToken);
    Task<bool> IsEmailExists(string email, string realm, CancellationToken cancellationToken);
    Task<bool> IsClaimExists(string name, string value, string realm, CancellationToken cancellationToken);
    Task<SearchResult<User>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
    void Update(User user);
    void Add(User user);
    void Remove(IEnumerable<User> users);
    Task BulkUpdate(List<UserClaim> userClaims);
    Task BulkUpdate(List<User> users);
    Task BulkUpdate(List<RealmUser> userRealms);
    Task BulkUpdate(List<GroupUser> groupUsers);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
