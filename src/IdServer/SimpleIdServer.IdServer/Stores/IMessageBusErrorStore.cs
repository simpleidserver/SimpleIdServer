// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IMessageBusErrorStore
{
    Task<MessageBusErrorMessage> Get(string id, CancellationToken cancellationToken);
    void Add(MessageBusErrorMessage message);
    void Delete(MessageBusErrorMessage message);
    Task<List<MessageBusErrorMessage>> GetAllByExternalId(List<string> externalIds, CancellationToken cancellationToken);
}
