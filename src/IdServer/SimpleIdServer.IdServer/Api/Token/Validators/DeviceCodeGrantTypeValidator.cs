// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
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
            if (string.IsNullOrWhiteSpace(deviceCode)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.DeviceCode));
            var authDeviceCode = await _deviceAuthCodeRepository.Query().SingleOrDefaultAsync(d => d.DeviceCode == deviceCode, cancellationToken);
            if (authDeviceCode == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.UnknownDeviceCode);
            if (authDeviceCode.Status == DeviceAuthCodeStatus.ISSUED) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIssuedDeviceCode);
            if (authDeviceCode.ExpirationDateTime <= currentDateTime) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.EXPIRED_TOKEN, Global.InvalidExpiredDeviceCode);
            if (authDeviceCode.ClientId != context.Client.Id) throw new OAuthException(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_REQUEST, Global.InvalidClientIdDeviceCode);
            if (authDeviceCode.NextAccessDateTime != null && authDeviceCode.NextAccessDateTime >= currentDateTime) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.SLOW_DOWN, Global.TooManyAuthRequest);
            if (authDeviceCode.Status == DeviceAuthCodeStatus.PENDING)
            {
                authDeviceCode.Next(_options.DeviceCodeInterval);
                throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.AUTHORIZATION_PENDING, Global.InvalidPendingDeviceCode);
            }

            return authDeviceCode;
        }
    }
}
