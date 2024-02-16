// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Users
{
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IRealmRepository _realmRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IBusControl _busControl;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ILogger<UsersController> _logger;
        private readonly IdServerHostOptions _options;

        public UsersController(
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IAuthenticationHelper authenticationHelper,
            IRealmRepository realmRepository,
            IGroupRepository groupRepository,
            IBusControl busControl,
            ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder,
            IRecurringJobManager recurringJobManager,
            ILogger<UsersController> logger,
            IOptions<IdServerHostOptions> options) : base(tokenRepository, jwtBuilder)
        {
            _userRepository = userRepository;
            _userSessionRepository = userSessionRepository;
            _authenticationHelper = authenticationHelper;
            _realmRepository = realmRepository;
            _groupRepository = groupRepository;
            _busControl = busControl;
            _recurringJobManager = recurringJobManager;
            _logger = logger;
            _options = options.Value;
        }

        #region Querying

        [HttpPost]
        public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                var result = await _userRepository.Search(prefix, request, cancellationToken);
                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchSessions([FromRoute] string prefix, string id, [FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            try
            {
                await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                var result = await _userSessionRepository.Search(id, prefix, request, cancellationToken);
                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                var user = await _userRepository.GetById(id, prefix, cancellationToken);
                if (user == null) return new NotFoundResult();
                return new OkObjectResult(user);
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResolveRoles([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                var user = await _userRepository.GetById(id, prefix, cancellationToken);
                if (user == null) return new NotFoundResult();
                var grpPathLst = user.Groups.SelectMany(g => g.Group.ResolveAllPath()).Distinct();
                var allGroups = await _groupRepository.Query()
                    .Include(g => g.Roles)
                    .AsNoTracking()
                    .Where(g => grpPathLst.Contains(g.FullPath))
                    .ToListAsync();
                var roles = allGroups.SelectMany(g => g.Roles).Select(r => r.Name).Distinct();
                return new OkObjectResult(roles);
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
        public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add user"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var realm = await _realmRepository.Query().FirstAsync(r => r.Name == prefix, cancellationToken);
                    await Validate();
                    var newUser = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = request.Name,
                        Firstname = request.Firstname,
                        Lastname = request.Lastname,
                        Email = request.Email,
                        EmailVerified = request.EmailVerified,
                        OAuthUserClaims = request.Claims?.Select(c => new UserClaim
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = c.Key,
                            Value = c.Value
                        }).ToList(),
                        UpdateDateTime = DateTime.UtcNow,
                        CreateDateTime = DateTime.UtcNow
                    };
                    newUser.Realms.Add(new RealmUser
                    {
                        Realm = realm
                    });
                    _userRepository.Add(newUser);
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Add user success");
                    await _busControl.Publish(new AddUserSuccessEvent
                    {
                        Realm = prefix,
                        Id = newUser.Id,
                        Name = newUser.Name,
                        Email = newUser.Email,
                        Firstname = newUser.Firstname,
                        Lastname = newUser.Lastname,
                        Claims = request.Claims
                    });
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(newUser),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Created
                    };
                }
                catch (OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }

            async Task Validate()
            {
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, UserNames.Name));
                if(await _userRepository.IsSubjectExists(request.Name, prefix, cancellationToken)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UserExists, request.Name));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Update user"))
            {
                try
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    user.UpdateEmail(request.Email);
                    user.UpdateName(request.Name);
                    user.UpdateLastname(request.Lastname);
                    user.EmailVerified = request.EmailVerified;
                    user.NotificationMode = request.NotificationMode ?? string.Empty;
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "User is updated");
                    await _busControl.Publish(new UpdateUserSuccessEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new UpdateUserFailureEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return BuildError(ex);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove user"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) return new NotFoundResult();
                    _userRepository.Remove(new List<User> { user });
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "User is removed");
                    await _busControl.Publish(new RemoveUserSuccessEvent
                    {
                        Realm = prefix,
                        Id = id,
                        Name = user.Name
                    });
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        #endregion

        #region Credentials

        [HttpPost]
        public async Task<IActionResult> AddCredential([FromRoute] string prefix, string id, [FromBody] AddUserCredentialRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add user's credential"))
            {
                try
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    if (request.Active)
                    {
                        foreach (var act in user.Credentials.Where(c => c.CredentialType == request.Credential.CredentialType))
                            act.IsActive = false;
                        request.Credential.IsActive = true;
                    }

                    request.Credential.Id = Guid.NewGuid().ToString();
                    user.Credentials.Add(request.Credential);
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "User's credential is added");
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(request.Credential),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Created
                    };
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCredential([FromRoute] string prefix, string id, string credentialId, [FromBody] UpdateUserCredentialRequest request, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Update credential"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    if (string.IsNullOrWhiteSpace(request.Value)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, UserCredentialNames.Value));
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) return new NotFoundResult();
                    var existingCredential = user.Credentials.SingleOrDefault(c => c.Id == credentialId);
                    if (existingCredential == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownUserCredential, credentialId));
                    existingCredential.Value = request.Value;
                    existingCredential.OTPAlg = request.OTPAlg;
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Credential is replaced");
                    await _busControl.Publish(new UpdateUserCredentialSuccessEvent
                    {
                        Realm = prefix,
                        Name = user.Name
                    });
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(existingCredential),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
                catch (OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCredential([FromRoute] string prefix, string id, string credentialId, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove credential"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) return new NotFoundResult();
                    var existingCredential = user.Credentials.SingleOrDefault(c => c.Id == credentialId);
                    if (existingCredential == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownUserCredential, credentialId));
                    user.Credentials.Remove(existingCredential);
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Credential is removed");
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> DefaultCredential([FromRoute] string prefix, string id, string credentialId, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Set default credential"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) return new NotFoundResult();
                    var existingCredential = user.Credentials.SingleOrDefault(c => c.Id == credentialId);
                    if (existingCredential == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownUserCredential, credentialId));
                    foreach (var cred in user.Credentials.Where(c => c.CredentialType == existingCredential.CredentialType))
                        existingCredential.IsActive = false;
                    existingCredential.IsActive = true;
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Default credential is set");
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }
            }
        }

        #endregion

        #region Claims

        [HttpPut]
        public async Task<IActionResult> UpdateClaims([FromRoute] string prefix, string id, [FromBody] UpdateUserClaimsRequest request, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Update claims"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    Validate();
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    Update(user, request);
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Claims are updated");
                    await _busControl.Publish(new UpdateUserClaimsSuccessEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new UpdateUserClaimsFailureEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return BuildError(ex);
                }

                void Validate()
                {
                    if (request == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                }

                void Update(User user, UpdateUserClaimsRequest request)
                {
                    user.OAuthUserClaims.Clear();
                    foreach (var cl in request.Claims)
                        user.OAuthUserClaims.Add(new UserClaim { Id = Guid.NewGuid().ToString(), Name = cl.Name, Value = cl.Value });
                }
            }
        }

        #endregion

        #region Groups

        [HttpPost]
        public async Task<IActionResult> AddGroup([FromRoute] string prefix, string id, string groupId, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add user's group"))
            {
                try
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    var newGroup = await _groupRepository.Query()
                        .Include(g => g.Realms)
                        .SingleOrDefaultAsync(a => a.Id == groupId && a.Realms.Any(r => r.RealmsName == prefix)); ;
                    if (newGroup == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownUserGroup, groupId));
                    user.Groups.Add(new GroupUser
                    {
                        GroupsId = newGroup.Id
                    });
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "User's group is added");
                    await _busControl.Publish(new AssignUserGroupSuccessEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(newGroup),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Created
                    };
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new AssignUserGroupFailureEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return BuildError(ex);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveGroup([FromRoute] string prefix, string id, string groupId, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove user's group"))
            {
                try
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    var assignedGroup = user.Groups.SingleOrDefault(g => g.GroupsId == groupId);
                    if (assignedGroup == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownUserGroup, groupId));
                    user.Groups.Remove(assignedGroup);
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "User's group is removed");
                    await _busControl.Publish(new RemoveUserGroupSuccessEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new RemoveUserGroupFailureEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return BuildError(ex);
                }
            }
        }

        #endregion

        #region Consents

        [HttpDelete]
        public async Task<IActionResult> RevokeConsent([FromRoute] string prefix, string id, string consentId, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Revoke user's consent"))
            {
                try
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    var consent = user.Consents.SingleOrDefault(c => c.Id == consentId);
                    if (consent == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownUserConsent, consentId));
                    user.Consents.Remove(consent);
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "User's consent is revoked");
                    await _busControl.Publish(new RevokeUserConsentSuccessEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new RevokeUserConsentFailureEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return BuildError(ex);
                }
            }
        }

        #endregion

        #region Sessions

        [HttpDelete]
        public async Task<IActionResult> RevokeSessions([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Revoke user's sessions"))
            {
                try
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    var sessions = await _userSessionRepository.GetActive(id, prefix, cancellationToken);
                    foreach (var session in sessions)
                        session.State = UserSessionStates.Rejected;
                    await _userSessionRepository.SaveChanges(cancellationToken);
                    _recurringJobManager.Trigger(nameof(UserSessionJob));
                    activity?.SetStatus(ActivityStatusCode.Ok, "User's sessions are revoked");
                    await _busControl.Publish(new RevokeUserSessionsSuccessEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return new NoContentResult();
                }
                catch(OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new RevokeUserSessionsFailureEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return BuildError(ex);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RevokeSession([FromRoute] string prefix, string id, string sessionId, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Revoke user's session"))
            {
                try
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    var session = await _userSessionRepository.GetById(sessionId, prefix, cancellationToken);
                    if (session == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownUserSession, sessionId));
                    session.State = UserSessionStates.Rejected;
                    await _userSessionRepository.SaveChanges(cancellationToken);
                    _recurringJobManager.Trigger(nameof(UserSessionJob));
                    activity?.SetStatus(ActivityStatusCode.Ok, "User's session is revoked");
                    await _busControl.Publish(new RevokeUserSessionSuccessEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new RevokeUserSessionFailureEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return BuildError(ex);
                }
            }
        }

        #endregion

        #region External Auth Providers

        [HttpPost]
        public async Task<IActionResult> UnlinkExternalAuthProvider([FromRoute] string prefix, string id, [FromBody] UnlinkExternalAuthProviderRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Unlink user's external authentication provider"))
            {
                try
                {
                    await CheckAccessToken(prefix, Constants.StandardScopes.Users.Name);
                    if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                    if (string.IsNullOrWhiteSpace(request.Scheme)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, UserExternalAuthProviderNames.Scheme));
                    if (string.IsNullOrWhiteSpace(request.Subject)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, UserExternalAuthProviderNames.Subject));
                    var user = await _userRepository.GetById(id, prefix, cancellationToken);
                    if (user == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownUser, id));
                    var externalAuthProvider = user.ExternalAuthProviders.SingleOrDefault(c => c.Subject == request.Subject && c.Scheme == request.Scheme);
                    if (externalAuthProvider == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.UnknownUserExternalAuthProvider);
                    user.ExternalAuthProviders.Remove(externalAuthProvider);
                    user.UpdateDateTime = DateTime.UtcNow;
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "User's external authentication provider is unlinked");
                    await _busControl.Publish(new UnlinkUserExternalAuthProviderSuccessEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    _logger.LogError(ex.ToString());
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new UnlinkUserExternalAuthProviderFailureEvent
                    {
                        Realm = prefix,
                        Id = id
                    });
                    return BuildError(ex);
                }
            }
        }

        #endregion
    }
}
