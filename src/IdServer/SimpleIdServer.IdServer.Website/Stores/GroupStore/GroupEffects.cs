// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.GroupStore
{
    public class GroupEffects
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly DbContextOptions<StoreDbContext> _options;

        public GroupEffects(IGroupRepository groupRepository, ProtectedSessionStorage sessionStorage, DbContextOptions<StoreDbContext> options) 
        {
            _groupRepository = groupRepository;
            _sessionStorage = sessionStorage;
            _options = options;
        }

        [EffectMethod]
        public async Task Handle(SearchGroupsAction action, IDispatcher dispatcher)
        {
            var realm = await GetRealm();
            IQueryable<Group> query = _groupRepository.Query().Include(c => c.Realms).Where(c => c.Realms.Any(r => r.Name == realm) && (!action.OnlyRoot || action.OnlyRoot && c.Name == c.FullPath)).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));
            else
                query = query.OrderBy(q => q.FullPath);

            var nb = query.Count();
            var groups = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchGroupsSuccessAction { Groups = groups, Count = nb });

            string SanitizeExpression(string expression) => expression.Replace("Group.", "").Replace("Value", "");
        }

        [EffectMethod]
        public async Task Handle(RemoveSelectedGroupsAction action, IDispatcher dispatcher)
        {
            var groups = new List<Group>();
            foreach(var fullPath in action.FullPathLst)
                groups.AddRange(await _groupRepository.Query().Where(g => g.FullPath.StartsWith(fullPath)).ToListAsync(CancellationToken.None));

            _groupRepository.DeleteRange(groups);
            await _groupRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedGroupsSuccessAction { FullPathLst = action.FullPathLst });
        }

        [EffectMethod]
        public async Task Handle(AddGroupAction action, IDispatcher dispatcher)
        {
            var fullPath = action.Name;
            if (!string.IsNullOrWhiteSpace(action.ParentId))
            {
                var parent = await _groupRepository.Query().SingleAsync(g => g.Id == action.ParentId);
                fullPath = $"{parent.FullPath}.{action.Name}";
            }

            var exists = await _groupRepository.Query().AnyAsync(g => g.FullPath == fullPath, CancellationToken.None);
            if(exists)
            {
                dispatcher.Dispatch(new AddGroupFailureAction { ErrorMessage = string.Format(Resources.Global.GroupAlreadyExists, action.Name) });
                return;
            }

            using (var dbContext = new StoreDbContext(_options))
            {
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
            var grp = await _groupRepository.Query().Include(m => m.Children).Include(m => m.Roles).AsNoTracking().SingleOrDefaultAsync(g => g.Id== action.Id, CancellationToken.None);
            if(grp == null)
            {
                dispatcher.Dispatch(new GetGroupFailureAction { ErrorMessage =  Resources.Global.UnknownGroup });
                return;
            }

            var rootGroup = grp;
            var fullPath = grp.FullPath;
            var splittedFullPath = fullPath.Split('.');
            if(splittedFullPath.Count() > 1)
                rootGroup = await _groupRepository.Query().Include(m => m.Children).AsNoTracking().SingleAsync(g => g.FullPath == splittedFullPath[0], CancellationToken.None);
            dispatcher.Dispatch(new GetGroupSuccessAction { Group = grp, RootGroup = rootGroup });
        }

        [EffectMethod]
        public async Task Handle(AddGroupRolesAction action, IDispatcher dispatcher)
        {
            using (var dbContext = new StoreDbContext(_options))
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
            var grp = await _groupRepository.Query().Include(m => m.Roles).SingleAsync(g => g.Id == action.Id, CancellationToken.None);
            grp.Roles = grp.Roles.Where(r => !action.RoleIds.Contains(r.Id)).ToList();
            await _groupRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RemoveSelectedGroupRolesSuccessAction
            {
                Id = action.Id,
                RoleIds = action.RoleIds
            });
        }

        private async Task<string> GetRealm()
        {
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
