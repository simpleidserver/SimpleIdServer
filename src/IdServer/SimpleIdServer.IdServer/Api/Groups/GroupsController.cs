// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(
            IGroupRepository groupRepository,
            IScopeRepository scopeRepository,
            IRealmRepository realmRepository,
            ITokenRepository tokenRepository,
            ITransactionBuilder transactionBuilder,
            IJwtBuilder jwtBuilder, 
            ILogger<GroupsController> logger) : base(tokenRepository, jwtBuilder)
        {
            _groupRepository = groupRepository;
            _scopeRepository = scopeRepository;
            _realmRepository = realmRepository;
            _transactionBuilder = transactionBuilder;
            _logger = logger;
        }

        #region Querying

        [HttpPost]
        public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchGroupsRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                var result = await _groupRepository.Search(prefix, request, cancellationToken);
                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                var result = await _groupRepository.Get(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownGroup, id));
                var splittedFullPath = result.FullPath.Split('.');
               var rootGroup = result;
                if (splittedFullPath.Count() > 1)
                    rootGroup = await _groupRepository.GetByStrictFullPath(prefix, splittedFullPath[0], cancellationToken);
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

        [HttpGet]
        public async Task<IActionResult> GetHierarchicalGroup([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                var result = await _groupRepository.Get(prefix, id, cancellationToken);
                if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownGroup, id));
                var children = await _groupRepository.GetAllByFullPath(prefix, id, result.FullPath, cancellationToken);
                return new OkObjectResult(new List<GetHierarchicalGroupResult>
                {
                    GetHierarchicalGroupResult.BuildRoot(children, result)
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
        public async Task<IActionResult> Delete([FromRoute] string prefix, [FromBody] RemoveGroupRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove group"))
            {
                try
                {
                    using (var transaction = _transactionBuilder.Build())
                    {
                        activity?.SetTag("realm", prefix);
                        await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                        var result = await _groupRepository.GetAllByFullPath(prefix, request.FullPath, cancellationToken);
                        _groupRepository.DeleteRange(result);
                        activity?.SetStatus(ActivityStatusCode.Ok, $"Groups {request.FullPath} are removed");
                        await transaction.Commit(cancellationToken);
                        return new NoContentResult();
                    }
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
        public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddGroupRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add group"))
            {
                try
                {
                    using (var transaction = _transactionBuilder.Build())
                    {
                        activity?.SetTag("realm", prefix);
                        await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                        var fullPath = request.Name;
                        if (!string.IsNullOrWhiteSpace(request.ParentGroupId))
                        {
                            var parent = await _groupRepository.Get(prefix, request.ParentGroupId, cancellationToken);
                            fullPath = $"{parent.FullPath}.{request.Name}";
                        }

                        var existingGroup = await _groupRepository.GetByStrictFullPath(prefix, fullPath, cancellationToken);
                        if (existingGroup != null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.GroupExists, fullPath));
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
                        grp.Realms.Add(new GroupRealm
                        {
                            RealmsName = prefix,
                            GroupsId = grp.Id
                        });
                        _groupRepository.Add(grp);
                        await transaction.Commit(cancellationToken);
                        activity?.SetStatus(ActivityStatusCode.Ok, $"Group {fullPath} is added");
                        return new ContentResult
                        {
                            StatusCode = (int)HttpStatusCode.Created,
                            Content = JsonSerializer.Serialize(grp).ToString(),
                            ContentType = "application/json"
                        };
                    }
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
        public async Task<IActionResult> AddRole([FromRoute] string prefix, string id, [FromBody] AddGroupRoleRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add group role"))
            {
                try
                {
                    using (var transaction = _transactionBuilder.Build())
                    {
                        activity?.SetTag("realm", prefix);
                        await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                        var result = await _groupRepository.Get(prefix, id, cancellationToken);
                        if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownGroup, id));
                        var scope = await _scopeRepository.GetByName(prefix, request.Scope, cancellationToken);
                        if (scope == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownScope, request.Scope));
                        result.Roles.Add(scope);
                        result.UpdateDateTime = DateTime.UtcNow;
                        _groupRepository.Update(result);
                        await transaction.Commit(cancellationToken);
                        activity?.SetStatus(ActivityStatusCode.Ok, $"Group role {request.Scope} is added");
                        return new ContentResult
                        {
                            StatusCode = (int)HttpStatusCode.Created,
                            Content = JsonSerializer.Serialize(scope).ToString(),
                            ContentType = "application/json"
                        };
                    }
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
        public async Task<IActionResult> RemoveRole([FromRoute] string prefix, string id, string roleId, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove group role"))
            {
                try
                {
                    using (var transaction = _transactionBuilder.Build())
                    {
                        activity?.SetTag("realm", prefix);
                        await CheckAccessToken(prefix, Constants.StandardScopes.Groups.Name);
                        var result = await _groupRepository.Get(prefix, id, cancellationToken);
                        if (result == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownGroup, id));
                        var role = result.Roles.SingleOrDefault(r => r.Id == roleId);
                        if (role == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownGroupRole, roleId));
                        result.Roles.Remove(role);
                        result.UpdateDateTime = DateTime.UtcNow;
                        _groupRepository.Update(result);
                        await transaction.Commit(cancellationToken);
                        activity?.SetStatus(ActivityStatusCode.Ok, $"Group role {roleId} is removed");
                        return new NoContentResult();
                    }
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
