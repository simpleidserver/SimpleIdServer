// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.ErrorMessages;

public class ErrorMessagesController : BaseController
{
    private readonly IMessageBusErrorStore _messageBusErrorStore;
    private readonly IBusControl _busControl;
    private readonly ITransactionBuilder _transactionBuilder;

    public ErrorMessagesController(
        IMessageBusErrorStore messageBusErrorStore,
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder,
        IBusControl busControl,
        ITransactionBuilder transactionBuilder) : base(tokenRepository, jwtBuilder)
    {
        _messageBusErrorStore = messageBusErrorStore;
        _busControl = busControl;
        _transactionBuilder = transactionBuilder;
    }

    [HttpGet]
    public async Task<IActionResult> Relaunch([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                var messageBusError = await _messageBusErrorStore.Get(id, cancellationToken);
                if (messageBusError == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownErrorMessage, id));
                await Relaunch(messageBusError, cancellationToken);
                _messageBusErrorStore.Delete(messageBusError);
                await transaction.Commit(cancellationToken);
                return new NoContentResult();
            }
            
        }
        catch(OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> RelaunchAllByExternalId([FromRoute] string prefix, [FromBody] RelaunchAllErrorMessagesByExternalIdRequest request, CancellationToken cancellationToken)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var errorMessages = await _messageBusErrorStore.GetAllByExternalId(new List<string> { request.ExternalId }, cancellationToken);
            foreach (var errorMessage in errorMessages)
            {
                await Relaunch(errorMessage, cancellationToken);
                _messageBusErrorStore.Delete(errorMessage);
            }

            await transaction.Commit(cancellationToken);
            return new NoContentResult();
        }
    }

    private async Task Relaunch(MessageBusErrorMessage errorMessage, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{errorMessage.QueueName}"));
        var typeCmd = Type.GetType(errorMessage.FullName);
        var cmd = JsonSerializer.Deserialize(errorMessage.Content, typeCmd);
        await sendEndpoint.Send(cmd, cancellationToken);
    }
}