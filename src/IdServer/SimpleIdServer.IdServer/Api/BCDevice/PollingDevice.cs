// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.BCDeviceRegistration
{
    public interface IDevice
    {
        string Type { get; }
        UserDevice Register(User authenticatedUser, BCDeviceRegistrationRequest request);
        Task Publish(PublishMessageRequest request, CancellationToken cancellationToken);
        Task<IEnumerable<PollingDeviceMessage>> GetLastUnreadMessages(User authenticatedUser, DateTime lastReceptionDateTime, CancellationToken cancellationToken);
    }

    public class PublishMessageRequest
    {
        public string DeviceId { get; set; }
        public string AuthReqId { get; set; }
        public string BindingMessage { get; set; }
        public string ClientId { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public IEnumerable<BCAuthorizePermission> Permissions { get; set; }
    }

    /// <summary>
    /// Device used to do polling.
    /// </summary>
    public class PollingDevice : IDevice
    {
        private readonly IPollingDeviceMessageRepository _repository;
        public string Type => DEVICE_TYPE;
        public const string DEVICE_TYPE = "polling";

        public PollingDevice(IPollingDeviceMessageRepository repository)
        {
            _repository = repository;
        }

        public UserDevice Register(User authenticatedUser, BCDeviceRegistrationRequest request)
        {
            var d = authenticatedUser.Devices.SingleOrDefault(d => d.Type == DEVICE_TYPE);
            if (d != null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.POLLING_DEVICE_ALREADY_REGISTERED);
            var device = new UserDevice { Id = Guid.NewGuid().ToString(), SerializedOptions = string.Empty, Type = DEVICE_TYPE };
            authenticatedUser.Devices.Add(device);
            return device;
        }

        public async Task Publish(PublishMessageRequest request, CancellationToken cancellationToken)
        {
            _repository.Add(new PollingDeviceMessage
            {
                AuthReqId = request.AuthReqId,
                BindingMessage = request.BindingMessage,
                DeviceId = request.DeviceId,
                Scopes = request.Scopes,
                ClientId = request.ClientId,
                ReceptionDateTime = DateTime.UtcNow
            });
            await _repository.SaveChanges(cancellationToken);
        }

        public async Task<IEnumerable<PollingDeviceMessage>> GetLastUnreadMessages(User authenticatedUser, DateTime lastReceptionDateTime, CancellationToken cancellationToken)
        {
            var d = authenticatedUser.Devices.SingleOrDefault(d => d.Type == DEVICE_TYPE);
            if (d == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.POLLING_DEVICE_NOT_REGISTERED);
            var res = await _repository.Query().AsNoTracking().OrderBy(d => d.ReceptionDateTime).Where(d => d.DeviceId == d.DeviceId && d.ReceptionDateTime > lastReceptionDateTime).ToListAsync(cancellationToken);
            return res;
        }
    }
}
