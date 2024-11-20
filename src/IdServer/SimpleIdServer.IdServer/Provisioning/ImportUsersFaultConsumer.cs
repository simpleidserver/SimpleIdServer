// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Provisioning
{
    public class ImportUsersFaultConsumer : IConsumer<Fault<ImportUsersCommand>>
    {
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IMessageBusErrorStore _messageBusErrorStore;

        public ImportUsersFaultConsumer(ITransactionBuilder transactionBuilder, IMessageBusErrorStore messageBusErrorStore)
        {
            _transactionBuilder = transactionBuilder;
            _messageBusErrorStore = messageBusErrorStore;
        }

        public async Task Consume(ConsumeContext<Fault<ImportUsersCommand>> context)
        {
            using (var transaction = _transactionBuilder.Build())
            {
                var type = typeof(ImportUsersCommand);
                var exceptions = context.Message.Exceptions == null ? new List<string>() : context.Message.Exceptions.Select(e => e.Message).ToList();
                var message = new MessageBusErrorMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = JsonSerializer.Serialize(context.Message.Message),
                    ExternalId = context.Message.Message.ProcessId,
                    FullName = type.AssemblyQualifiedName,
                    Name = type.Name,
                    Exceptions = exceptions,
                    ReceivedDateTime = DateTime.UtcNow,
                    QueueName = ImportUsersConsumer.Queuename
                };
                _messageBusErrorStore.Add(message);
                await transaction.Commit(context.CancellationToken);
            }
        }
    }
}