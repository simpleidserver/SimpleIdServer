// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Events;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Store;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Statistics;

public class StatisticsController : BaseController
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IAuditEventRepository _auditEventRepository;

    public StatisticsController(IUserRepository userRepository, IClientRepository clientRepository, IAuditEventRepository auditEventRepository)
    {
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _auditEventRepository = auditEventRepository;

    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var currentDate = DateTime.UtcNow.Date;
        var nbUsers = await _userRepository
            .Query()
            .Include(u => u.Realms)
            .CountAsync(u => u.Realms.Any(r => r.RealmsName == prefix));
        var nbClients = await _clientRepository
        .Query()
        .Include(u => u.Realms)
            .CountAsync(u => u.Realms.Any(r => r.Name == prefix));
        var nbValidAuthentications = await _auditEventRepository
            .Query()
            .CountAsync(e => e.CreateDateTime >= currentDate && e.EventName == nameof(UserLoginSuccessEvent) && e.Realm == prefix);
        var nbInvalidAuthentications = await _auditEventRepository
            .Query()
            .CountAsync(e => e.CreateDateTime >= currentDate && e.EventName == nameof(UserLoginFailureEvent) && e.Realm == prefix);
        return new OkObjectResult(new StatisticResult
        {
           InvalidAuthentications = nbInvalidAuthentications,
           NbClients = nbClients,
           NbUsers = nbUsers, 
           ValidAuthentications = nbValidAuthentications
        });
    }
}