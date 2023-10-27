// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Did;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
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
        private readonly IRealmRepository _realmRepository;
        private readonly IBusControl _busControl;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEnumerable<IDIDGenerator> _generators;

        public UsersController(IUserRepository userRepository, IRealmRepository realmRepository, IBusControl busControl, IJwtBuilder jwtBuilder, IEnumerable<IDIDGenerator> generators)
        {
            _userRepository = userRepository;
            _realmRepository = realmRepository;
            _busControl = busControl;
            _jwtBuilder = jwtBuilder;
            _generators = generators;
        }

        /// <summary>
        /// Add a user.
        /// </summary>
        /// <param name="prefix">Realm</param>
        /// <param name="request">User parameters</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OAuthException"></exception>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add user"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    CheckAccessToken(prefix, Constants.StandardScopes.Users.Name, _jwtBuilder);
                    var realm = await _realmRepository.Query().FirstAsync(r => r.Name == prefix, cancellationToken);
                    await Validate();
                    var newUser = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = request.Name,
                        Firstname = request.Firstname,
                        Lastname = request.Lastname,
                        Email = request.Email,
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
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, UserNames.Name));
                if ((await _userRepository.GetAll(us => us.AsNoTracking().Where(u => u.Name == request.Name).ToListAsync())).Any()) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.USER_EXISTS, request.Name));
            }
        }

        /// <summary>
        /// Get a user.
        /// </summary>
        /// <param name="prefix">Realm</param>
        /// <param name="id">User's identifier</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                CheckAccessToken(prefix, Constants.StandardScopes.Users.Name, _jwtBuilder);
                var user = await _userRepository.Get(us => us.Include(u => u.OAuthUserClaims).Include(u => u.Credentials).Include(u => u.Realms).AsNoTracking().FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.RealmsName == prefix), cancellationToken));
                if (user == null) return new NotFoundResult();
                return new OkObjectResult(user);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        /// <summary>
        /// Remove a user.
        /// </summary>
        /// <param name="prefix">Realm</param>
        /// <param name="id">User's identifier</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove user"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    CheckAccessToken(prefix, Constants.StandardScopes.Users.Name, _jwtBuilder);
                    var user = await _userRepository.Get(us => us.Include(u => u.Realms).FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.RealmsName == prefix), cancellationToken));
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

        /// <summary>
        /// Add or update a credential.
        /// </summary>
        /// <param name="prefix">Realm.</param>
        /// <param name="id">User's identifier.</param>
        /// <param name="request">User's credential.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OAuthException"></exception>
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ReplaceCredential([FromRoute] string prefix, string id, [FromBody] ReplaceCredentialRequest request, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Replace credential"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    CheckAccessToken(prefix, Constants.StandardScopes.Users.Name, _jwtBuilder);
                    Validate();
                    var user = await _userRepository.Get(us => us.Include(u => u.Realms).Include(u => u.Credentials).FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.RealmsName == prefix), cancellationToken));
                    if (user == null) return new NotFoundResult();
                    Update(user);
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Credential is replaced");
                    await _busControl.Publish(new UpdateUserCredentialSuccessEvent
                    {
                        Realm = prefix,
                        Name = user.Name,
                        Type = request.Type
                    });
                    return new NoContentResult();
                }
                catch (OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(ex);
                }

                void Validate()
                {
                    if(string.IsNullOrWhiteSpace(request.Type)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, UserCredentialNames.Type));
                    if (string.IsNullOrWhiteSpace(request.Value)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, UserCredentialNames.Value));
                    if (request.Type != UserCredential.PWD) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL_TYPE, request.Type));
                }

                void Update(User user)
                {
                    var cred = user.Credentials.FirstOrDefault(c => c.CredentialType == request.Type);
                    if(cred == null)
                    {
                        cred = new UserCredential
                        {
                            Id = Guid.NewGuid().ToString(),
                            CredentialType = request.Type
                        };
                        user.Credentials.Add(cred);
                    }
                    switch(request.Type)
                    {
                        case UserCredential.PWD:
                            cred.Value = PasswordHelper.ComputeHash(request.Value);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Generate decentralized identity.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OAuthException"></exception>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GenerateDecentralizedIdentity([FromRoute] string prefix, string id, [FromBody] GenerateDecentralizedIdentityRequest request, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Generate decentralized identity"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    CheckAccessToken(prefix, Constants.StandardScopes.Users.Name, _jwtBuilder);
                    Validate();
                    var user = await _userRepository.Get(us => us.Include(u => u.Realms).Include(u => u.CredentialOffers).FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.RealmsName == prefix), cancellationToken));
                    if (user == null) return BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_USER, id));
                    var generator = _generators.FirstOrDefault(g => g.Method == request.Method);
                    if (generator == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.INVALID_DECENTRALIZED_IDENTITY_METHOD, request.Method));
                    var generationResult = await generator.Generate(request.Parameters, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(generationResult.ErrorMessage)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, generationResult.ErrorMessage);
                    user.Did = generationResult.DID;
                    user.DidPrivateHex = generationResult.PrivateKey;
                    user.CredentialOffers.Clear();
                    await _userRepository.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Decentralized Identity is generated");
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(new GenerateDecentralizedIdentifierResult
                        {
                            DID = generationResult.DID,
                            PrivateKey = generationResult.PrivateKey
                        }),
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

            void Validate()
            {
                if (request == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
                if (string.IsNullOrWhiteSpace(request.Method)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, GenerateDecentralizedIdentityRequestNames.Method));
            }
        }
    }
}
