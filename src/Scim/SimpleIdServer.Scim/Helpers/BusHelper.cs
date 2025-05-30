// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using Microsoft.Extensions.Options;
using SimpleIdServer.Scim.ExternalEvents;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers
{
    public interface IBusHelper
    {
        Task Publish<T>(T evt, CancellationToken cancellationToken = default) where T : IntegrationEvent;
    }

    public class BusHelper : IBusHelper
    {
        private readonly ScimHostOptions _options;
        private readonly IBusControl _busControl;
        private readonly IMessageDataRepository _messageDataRepository;

        public BusHelper(
            IOptionsMonitor<ScimHostOptions> options, 
            IBusControl busControl, 
            IMessageDataRepository messageDataRepository)
        {
            _options = options.CurrentValue;
            _busControl = busControl;
            _messageDataRepository = messageDataRepository;
        }

        public async Task Publish<T>(T evt, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            if (!_options.IsBigMessagePublished)
            {
                await _busControl.Publish(evt);
                return;
            }

            var bigPayload = await _messageDataRepository.PutBytes(Serialize(evt), cancellationToken);
            var message = new BigMessage
            {
                Name = typeof(T).Name,
                Payload = bigPayload
            };
            await _busControl.Publish(message, cancellationToken);
        }

        public static byte[] Serialize(object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
