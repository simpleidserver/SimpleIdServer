// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Groups
{
    public class GroupsController : BaseController
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IScopeRepository _scopeRepository;
        private readonly IRealmRepository _realmRepository;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(
            IGroupRepository groupRepository,
            IScopeRepository scopeRepository,
            IRealmRepository realmRepository,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder, 
            ILogger<GroupsController> logger) : base(tokenRepository, jwtBuilder)
        {
            _groupRepository = groupRepository;
            _scopeRepository = scopeRepository;
            _realmRepository = realmRepository;
            _logger = logger;
        }

        #region Querying

        [HttpPost]
        public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchGroupsRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                IQueryable<Group> query = _groupRepository.Query()
                    .Include(c => c.Realms)
                    .Where(c => c.Realms.Any(r => r.Name == prefix) && (!request.OnlyRoot || request.OnlyRoot && c.Name == c.FullPath))
                    .AsNoTracking();
                if (!string.IsNullOrWhiteSpace(request.Filter))
                    query = query.Where(request.Filter);

                if (!string.IsNullOrWhiteSpace(request.OrderBy))
                    query = query.OrderBy(request.OrderBy);
                else
                    query = query.OrderBy(q => q.FullPath);

                var nb = query.Count();
                var groups = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
                return new OkObjectResult(new SearchResult<Group>
                {
                    Count = nb,
                    Content = groups
                });
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string prefix, string id)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                var result = await _groupRepository.Query()
                    .Include(c => c.Realms)
                    .Include(c => c.Children)
                    .Include(c => c.Roles)
                    .AsNoTracking()
                    .SingleAsync(g => g.Realms.Any(r => r.Name == prefix) && g.Id == id);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_GROUP, id));
                var splittedFullPath = result.FullPath.Split('.');
               var rootGroup = result;
                if (splittedFullPath.Count() > 1)
                    rootGroup = await _groupRepository.Query()
                        .Include(c => c.Realms)
                        .Include(c => c.Children)
                        .AsNoTracking()
                        .SingleAsync(g => g.FullPath == splittedFullPath[0], CancellationToken.None);
                return new OkObjectResult(new GetGroupResult
                {
                    Target = result,
                    Root = rootGroup
                });
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(ex);
            }
        }


        #endregion

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] string prefix, [FromBody] RemoveGroupRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove group"))
            {
                try
                {
                    activity?.SetTag("realm", prefix);
                    await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                    var result = await _groupRepository.Query()
                        .Include(c => c.Realms)
                        .Where(g => g.FullPath.StartsWith(request.FullPath) && g.Realms.Any(r => r.Name == prefix))
                        .ToListAsync();
                    _groupRepository.DeleteRange(result);
                    activity?.SetStatus(ActivityStatusCode.Ok, $"Groups {request.FullPath} are removed");
                    await _groupRepository.SaveChanges(CancellationToken.None);
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddGroupRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add group"))
            {
                try
                {
                    activity?.SetTag("realm", prefix);
                    await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                    var fullPath = request.Name;
                    if (!string.IsNullOrWhiteSpace(request.ParentGroupId))
                    {
                        var parent = await _groupRepository.Query().AsNoTracking().SingleAsync(g => g.Id == request.ParentGroupId);
                        fullPath = $"{parent.FullPath}.{request.Name}";
                    }

                    var groupAlreadyExists = await _groupRepository
                        .Query()
                        .Include(g => g.Realms)
                        .AnyAsync(g => g.Realms.Any(r => r.Name == prefix) && g.FullPath == fullPath);
                    if (groupAlreadyExists) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.GROUP_EXISTS, fullPath));
                    var realm = await _realmRepository.Query().SingleAsync(r => r.Name == prefix);
                    var grp = new Group
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = request.Name,
                        FullPath = fullPath,
                        ParentGroupId = request.ParentGroupId,
                        Description = request.Description,
                        CreateDateTime = DateTime.UtcNow,
                        UpdateDateTime = DateTime.UtcNow
                    };
                    grp.Realms.Add(realm);
                    _groupRepository.Add(grp);
                    await _groupRepository.SaveChanges(CancellationToken.None);
                    activity?.SetStatus(ActivityStatusCode.Ok, $"Group {fullPath} is added");
                    return new ContentResult
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Content = JsonSerializer.Serialize(grp).ToString(),
                        ContentType = "application/json"
                    };
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        #endregion

        #region Roles

        [HttpPost]
        public async Task<IActionResult> AddRole([FromRoute] string prefix, string id, [FromBody] AddGroupRoleRequest request)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add group role"))
            {
                try
                {
                    activity?.SetTag("realm", prefix);
                    await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                    var result = await _groupRepository.Query()
                        .Include(c => c.Realms)
                        .Include(c => c.Roles)
                        .SingleOrDefaultAsync(g => g.Realms.Any(r => r.Name == prefix) && g.Id == id);
                    if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_GROUP, id));
                    var scope = await _scopeRepository.Query()
                        .Include(s => s.Realms)
                        .SingleOrDefaultAsync(s => s.Name == request.Scope && s.Realms.Any(r => r.Name == prefix));
                    if(scope == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_SCOPE, request.Scope));
                    result.Roles.Add(scope);
                    result.UpdateDateTime = DateTime.UtcNow;
                    await _groupRepository.SaveChanges(CancellationToken.None);
                    activity?.SetStatus(ActivityStatusCode.Ok, $"Group role {request.Scope} is added");
                    return new ContentResult
                    {
                        StatusCode = (int)HttpStatusCode.Created,
                        Content = JsonSerializer.Serialize(scope).ToString(),
                        ContentType = "application/json"
                    };
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveRole([FromRoute] string prefix, string id, string roleId)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove group role"))
            {
                try
                {
                    activity?.SetTag("realm", prefix);
                    await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                    var result = await _groupRepository.Query()
                        .Include(c => c.Realms)
                        .Include(c => c.Roles)
                        .SingleOrDefaultAsync(g => g.Realms.Any(r => r.Name == prefix) && g.Id == id);
                    if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_GROUP, id));
                    var role = result.Roles.SingleOrDefault(r => r.Id == roleId);
                    if (role == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_GROUP_ROLE, roleId));
                    result.Roles.Remove(role);
                    result.UpdateDateTime = DateTime.UtcNow;
                    await _groupRepository.SaveChanges(CancellationToken.None);
                    activity?.SetStatus(ActivityStatusCode.Ok, $"Group role {roleId} is removed");
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        #endregion
    }
}
