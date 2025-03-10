// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultMessageBusErrorStore : IMessageBusErrorStore
{
    private readonly List<MessageBusErrorMessage> _errors;

    public DefaultMessageBusErrorStore(List<MessageBusErrorMessage> errors)
    {
        _errors = errors;
    }

    public void Add(MessageBusErrorMessage message)
    {
        _errors.Add(message);
    }

    public void Delete(MessageBusErrorMessage message)
    {
        _errors.Remove(message);
    }

    public Task<MessageBusErrorMessage> Get(string id, CancellationToken cancellationToken)
    {
        var message = _errors.SingleOrDefault(m => m.Id == id);
        return Task.FromResult(message);
    }

    public Task<List<MessageBusErrorMessage>> GetAllByExternalId(List<string> externalIds, CancellationToken cancellationToken)
    {
        var messages = _errors.Where(m => externalIds.Contains(m.ExternalId)).ToList();
        return Task.FromResult(messages);
    }
}
