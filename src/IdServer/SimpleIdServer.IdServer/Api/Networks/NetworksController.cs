// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Did;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Exceptions;
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

namespace SimpleIdServer.IdServer.Api.Networks
{
    [AllowAnonymous]
    public class NetworksController : BaseController
    {
        private readonly IIdentityDocumentConfigurationStore _store;
        private readonly IEnumerable<IContractDeploy> _contractDeploys;

        public NetworksController(ITokenRepository tokenRepository,
            IJwtBuilder jwtBuilder, 
            IIdentityDocumentConfigurationStore store, 
            IEnumerable<IContractDeploy> contractDeploys) : base(tokenRepository, jwtBuilder)
        {
            _store = store;
            _contractDeploys = contractDeploys;
        }

        /// <summary>
        /// Get all the networks.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAll([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Constants.StandardScopes.Networks.Name);
                var didConfigurations = await _store.Query().OrderByDescending(c => c.UpdateDateTime).ToListAsync(cancellationToken);
                return new OkObjectResult(didConfigurations);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        /// <summary>
        /// Remove one network.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Remove([FromRoute] string prefix, string name, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                await CheckAccessToken(prefix, Constants.StandardScopes.Networks.Name);
                var didConfiguration = await _store.Query().SingleAsync(s => s.Name == name, cancellationToken);
                if (didConfiguration == null) return this.BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_NETWORK, name));
                _store.Remove(didConfiguration);
                await _store.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        /// <summary>
        /// Add network.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OAuthException"></exception>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] AddNetworkRequest request, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Add network"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Constants.StandardScopes.Networks.Name);
                    Validate();
                    if (await _store.Query().AnyAsync(s => s.Name == request.Name, cancellationToken)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_NETWORK_NAME);
                    var networkConfiguration = new NetworkConfiguration
                    {
                        RpcUrl = request.RpcUrl,
                        Name = request.Name,
                        PrivateAccountKey = request.PrivateAccountKey,
                        CreateDateTime = DateTime.UtcNow,
                        UpdateDateTime = DateTime.UtcNow
                    };
                    _store.Add(networkConfiguration);
                    await _store.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, $"Network {request.Name} has been added");
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(networkConfiguration),
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
                if (string.IsNullOrWhiteSpace(request.Name)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, NetworkConfigurationNames.Name));
                if (string.IsNullOrWhiteSpace(request.RpcUrl)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, NetworkConfigurationNames.RpcUrl));
                if (string.IsNullOrWhiteSpace(request.PrivateAccountKey)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, NetworkConfigurationNames.PrivateAccountKey));
            }
        }

        /// <summary>
        /// Deploy a contract.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Deploy([FromRoute] string prefix, string name, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Deploy a contract"))
            {
                try
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Constants.StandardScopes.Networks.Name);
                    var network = await _store.Query().FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
                    if (network == null) return this.BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_NETWORK, name));
                    if (!string.IsNullOrWhiteSpace(network.ContractAdr)) return this.BuildError(HttpStatusCode.InternalServerError, ErrorCodes.INVALID_NETWORK, ErrorMessages.CONTRACT_ALREADY_DEPLOYED);
                    var contractDeploy = _contractDeploys.SingleOrDefault();
                    if (contractDeploy == null) return this.BuildError(HttpStatusCode.InternalServerError, ErrorCodes.UNEXPECTED_ERROR, ErrorMessages.NO_CONTRACT);
                    var result = await contractDeploy.Deploy(network);
                    network.ContractAdr = result;
                    network.UpdateDateTime = DateTime.UtcNow;
                    await _store.SaveChanges(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Contract is deployed");
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(new ContractDeployResult
                        {
                            ContractAdr = result
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
        }
    }
}
