// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Validators
{
    public interface IDeviceCodeGrantTypeValidator
    {
        Task<DeviceAuthCode> Validate(HandlerContext context, CancellationToken cancellationToken);
    }

    public class DeviceCodeGrantTypeValidator : IDeviceCodeGrantTypeValidator
    {
        private readonly IDeviceAuthCodeRepository _deviceAuthCodeRepository;
        private readonly IdServerHostOptions _options;

        public DeviceCodeGrantTypeValidator(IDeviceAuthCodeRepository deviceAuthCodeRepository, IOptions<IdServerHostOptions> options)
        {
            _deviceAuthCodeRepository = deviceAuthCodeRepository;
            _options = options.Value;
        }

        public async Task<DeviceAuthCode> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var currentDateTime = DateTime.UtcNow;
            var deviceCode = context.Request.RequestData.GetDeviceCode();
            if (string.IsNullOrWhiteSpace(deviceCode)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, TokenRequestParameters.DeviceCode));
            var authDeviceCode = await _deviceAuthCodeRepository.Query().SingleAsync(d => d.DeviceCode == deviceCode, cancellationToken);
            if (authDeviceCode == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.UNKNOWN_DEVICE_CODE);
            if (authDeviceCode.Status == DeviceAuthCodeStatus.ISSUED) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_ISSUED_DEVICE_CODE);
            if (authDeviceCode.ExpirationDateTime <= currentDateTime) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.EXPIRED_TOKEN, ErrorMessages.INVALID_EXPIRED_DEVICE_CODE);
            if (authDeviceCode.ClientId != context.Client.ClientId) throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_CLIENT_ID_DEVICE_CODE);
            if (authDeviceCode.NextAccessDateTime != null && authDeviceCode.NextAccessDateTime >= currentDateTime) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.SLOW_DOWN, ErrorMessages.TOO_MANY_AUTH_REQUEST);
            if (authDeviceCode.Status == DeviceAuthCodeStatus.PENDING)
            {
                authDeviceCode.Next(_options.DeviceCodeInterval);
                throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.AUTHORIZATION_PENDING, ErrorMessages.INVALID_PENDING_DEVICE_CODE);
            }

            return authDeviceCode;
        }
    }
}
