// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.DeviceAuthorization
{
    public interface IDeviceAuthorizationRequestHandler
    {
        Task<JsonObject> Handle(HandlerContext context, CancellationToken cancellationToken);
    }

    public class DeviceAuthorizationRequestHandler : IDeviceAuthorizationRequestHandler
    {
        private readonly IDeviceAuthorizationRequestValidator _validator;
        private readonly IDeviceAuthCodeRepository _deviceAuthCodeRepository;
        private readonly IdServerHostOptions _options;

        public DeviceAuthorizationRequestHandler(IDeviceAuthorizationRequestValidator validator, IDeviceAuthCodeRepository deviceAuthCodeRepository, IOptions<IdServerHostOptions> options)
        {
            _validator = validator;
            _deviceAuthCodeRepository = deviceAuthCodeRepository;
            _options = options.Value;
        }

        public virtual async Task<JsonObject> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            await _validator.Validate(context, cancellationToken);
            var deviceCode = Guid.NewGuid().ToString();
            var userCode = GenerateUserCode();
            var scopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            _deviceAuthCodeRepository.Add(DeviceAuthCode.Create(deviceCode, userCode, context.Client.Id, scopes, _options.DeviceCodeExpirationInSeconds));
            await _deviceAuthCodeRepository.SaveChanges(cancellationToken);
            var verificationUri = $"{context.Request.IssuerName}{context.UrlHelper.Action("Index", "Device")}";
            var verificationUriComplete = $"{context.Request.IssuerName}{context.UrlHelper.Action("Index", "Device", new { userCode = userCode })}";
            return new JsonObject
            {
                { DeviceAuthorizationNames.DeviceCode, deviceCode },
                { DeviceAuthorizationNames.UserCode, userCode },
                { DeviceAuthorizationNames.VerificationUri, verificationUri },
                { DeviceAuthorizationNames.VerificationUriComplete, verificationUriComplete },
                { DeviceAuthorizationNames.ExpiresIn, _options.DeviceCodeExpirationInSeconds },
                { DeviceAuthorizationNames.Interval, _options.DeviceCodeInterval }
            };
        }

        protected virtual string GenerateUserCode()
        {
            var rnd = new Random();
            return rnd.Next(1000, 9999).ToString();
        }
    }
}
