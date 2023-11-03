// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    public class GroupEffects
    {
        private readonly IDbContextFactory<StoreDbContext> _factory;
        private readonly IdServerWebsiteOptions _options;
        private readonly ProtectedSessionStorage _sessionStorage;

        public GroupEffects(IDbContextFactory<StoreDbContext> factory, IOptions<IdServerWebsiteOptions> options, ProtectedSessionStorage sessionStorage) 
        {
            _factory = factory;
            _options = options.Value;
            _sessionStorage = sessionStorage;
        }

        [EffectMethod]
        public async Task Handle(SearchGroupsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            using (var dbContext = _factory.CreateDbContext())
            {
                IQueryable<Group> query = dbContext.Groups.Include(c => c.Realms).Where(c => c.Realms.Any(r => r.Name == realm) && (!action.OnlyRoot || action.OnlyRoot && c.Name == c.FullPath)).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(action.Filter))
                    query = query.Where(SanitizeExpression(action.Filter));

                if (!string.IsNullOrWhiteSpace(action.OrderBy))
                    query = query.OrderBy(SanitizeExpression(action.OrderBy));
                else
                    query = query.OrderBy(q => q.FullPath);

                var nb = query.Count();
                var groups = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
                dispatcher.Dispatch(new SearchGroupsSuccessAction { Groups = groups, Count = nb });
            }

            string SanitizeExpression(string expression) => expression.Replace("Group.", "").Replace("Value", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedGroupsAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var groups = new List<Group>();
                foreach (var fullPath in action.FullPathLst)
                    groups.AddRange(await dbContext.Groups.Where(g => g.FullPath.StartsWith(fullPath)).ToListAsync(CancellationToken.None));

                dbContext.Groups.RemoveRange(groups);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new RemoveSelectedGroupsSuccessAction { FullPathLst = action.FullPathLst });
            }
        }

        [EffectMethod]
        public async Task Handle(AddGroupAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var fullPath = action.Name;
                if (!string.IsNullOrWhiteSpace(action.ParentId))
                {
                    var parent = await dbContext.Groups.SingleAsync(g => g.Id == action.ParentId);
                    fullPath = $"{parent.FullPath}.{action.Name}";
                }

                var exists = await dbContext.Groups.AnyAsync(g => g.FullPath == fullPath, CancellationToken.None);
                if (exists)
                {
                    dispatcher.Dispatch(new AddGroupFailureAction { ErrorMessage = string.Format(Resources.Global.GroupAlreadyExists, action.Name) });
                    return;
                }

                var id = Guid.NewGuid().ToString();
                var realm = await GetRealm();
                var activeRealm = await dbContext.Realms.FirstAsync(r => r.Name == realm);
                var grp = new Group
                {
                    Id = id,
                    Name = action.Name,
                    FullPath = fullPath,
                    ParentGroupId = action.ParentId,
                    Description = action.Description,
                    CreateDateTime = DateTime.UtcNow,
                    UpdateDateTime = DateTime.UtcNow
                };
                grp.Realms.Add(activeRealm);
                dbContext.Groups.Add(grp);
                await dbContext.SaveChangesAsync(CancellationToken.None);
                dispatcher.Dispatch(new AddGroupSuccessAction
                {
                    Description = action.Description,
                    Id = id,
                    Name = action.Name,
                    ParentGroupId = action.ParentId
                });
            }
        }

        [EffectMethod]
        public async Task Handle(GetGroupAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var grp = await dbContext.Groups.Include(m => m.Children).Include(m => m.Roles).AsNoTracking().SingleOrDefaultAsync(g => g.Id == action.Id, CancellationToken.None);
                if (grp == null)
                {
                    dispatcher.Dispatch(new GetGroupFailureAction { ErrorMessage = Resources.Global.UnknownGroup });
                    return;
                }

                var rootGroup = grp;
                var fullPath = grp.FullPath;
                var splittedFullPath = fullPath.Split('.');
                if (splittedFullPath.Count() > 1)
                    rootGroup = await dbContext.Groups.Include(m => m.Children).AsNoTracking().SingleAsync(g => g.FullPath == splittedFullPath[0], CancellationToken.None);
                dispatcher.Dispatch(new GetGroupSuccessAction { Group = grp, RootGroup = rootGroup });
            }
        }

        [EffectMethod]
        public async Task Handle(AddGroupRolesAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var grp = await dbContext.Groups.Include(g => g.Roles).SingleAsync(g => g.Id == action.GroupId);
                var roles = await dbContext.Scopes.Where(s => action.ScopeNames.Contains(s.Name)).ToListAsync();
                foreach (var role in roles)
                    grp.Roles.Add(role);

                await dbContext.SaveChangesAsync();
                dispatcher.Dispatch(new AddGroupRolesSuccessAction { Roles = roles });
            }
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedGroupRolesAction action, IDispatcher dispatcher)
        {
            using (var dbContext = _factory.CreateDbContext())
            {
                var grp = await dbContext.Groups.Include(m => m.Roles).SingleAsync(g => g.Id == action.Id, CancellationToken.None);
                grp.Roles = grp.Roles.Where(r => !action.RoleIds.Contains(r.Id)).ToList();
                await dbContext.SaveChangesAsync();
                dispatcher.Dispatch(new RemoveSelectedGroupRolesSuccessAction
                {
                    Id = action.Id,
                    RoleIds = action.RoleIds
                });
            }
        }

        private async Task<string> GetRealm()
        {
            if (!_options.IsReamEnabled) return SimpleIdServer.IdServer.Constants.DefaultRealm;
            var realm = await _sessionStorage.GetAsync<string>("realm");
            var realmStr = !string.IsNullOrWhiteSpace(realm.Value) ? realm.Value : SimpleIdServer.IdServer.Constants.DefaultRealm;
            return realmStr;
        }
    }

    public class SearchGroupsAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
        public bool OnlyRoot { get; set; } = true;
    }

    public class SearchGroupsSuccessAction
    {
        public IEnumerable<Group> Groups { get; set; } = new List<Group>();
        public int Count { get; set; }
    }

    public class ToggleAllGroupSelectionAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleGroupSelectionAction
    {
        public bool IsSelected { get; set; }
        public string GroupId { get; set; }
    }

    public class RemoveSelectedGroupsAction
    {
        public IEnumerable<string> FullPathLst { get; set; }
    }

    public class RemoveSelectedGroupsSuccessAction
    {
        public IEnumerable<string> FullPathLst { get; set; }
    }

    public class AddGroupAction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentId { get; set; }
    }

    public class AddGroupFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class AddGroupSuccessAction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentGroupId { get; set; }
    }

    public class GetGroupAction
    {
        public string Id { get; set; }
    }

    public class GetGroupSuccessAction
    {
        public Group Group { get; set; }
        public Group RootGroup { get; set; }
    }

    public class GetGroupFailureAction
    {
        public string ErrorMessage { get; set; }
    }

    public class SelectAllGroupMembersAction
    {
        public bool IsSelected { get; set; }
    }

    public class SelectGroupMemberAction
    {
        public string MemberId { get; set; }
        public bool IsSelected { get; set; }
    }
    
    public class ToggleAllGroupRolesAction
    {
        public bool IsSelected { get; set; }
    }

    public class ToggleGroupRoleAction
    {
        public bool IsSelected { get; set; }
        public string Id { get; set; }
    }

    public class RemoveSelectedGroupRolesAction
    {
        public string Id { get; set; }
        public IEnumerable<string> RoleIds { get; set; }
    }

    public class RemoveSelectedGroupRolesSuccessAction
    {
        public string Id { get; set; }
        public IEnumerable<string> RoleIds { get; set; }
    }

    public class AddGroupRolesAction
    {
        public string GroupId { get; set; } = null!;
        public IEnumerable<string> ScopeNames { get; set; } = new List<string>();
    }

    public class AddGroupRolesSuccessAction
    {
        public IEnumerable<Scope> Roles { get; set; }
    }
}
