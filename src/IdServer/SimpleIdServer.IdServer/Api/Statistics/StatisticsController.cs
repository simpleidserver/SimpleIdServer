// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Statistics;

public class StatisticsController : BaseController
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public StatisticsController(
        IUserRepository userRepository, 
        IClientRepository clientRepository, 
        IAuditEventRepository auditEventRepository,
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _auditEventRepository = auditEventRepository;

    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var currentDate = DateTime.UtcNow.Date;
        var nbUsers = await _userRepository.NbUsers(prefix, cancellationToken);
        var nbClients = await _clientRepository.NbClients(prefix, cancellationToken);
        var nbValidAuthentications = await _auditEventRepository.NbValidAuthentications(prefix, currentDate, cancellationToken);
        var nbInvalidAuthentications = await _auditEventRepository.NbInvalidAuthentications(prefix, currentDate, cancellationToken);
        return new OkObjectResult(new StatisticResult
        {
           InvalidAuthentications = nbInvalidAuthentications,
           NbClients = nbClients,
           NbUsers = nbUsers, 
           ValidAuthentications = nbValidAuthentications
        });
    }
}