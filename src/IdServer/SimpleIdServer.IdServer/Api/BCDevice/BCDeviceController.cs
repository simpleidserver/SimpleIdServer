// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api.BCDeviceRegistration;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.BCDevice
{
    public class BCDeviceController : BaseController
    {
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IUserRepository _userRepository;
        private readonly IEnumerable<IDevice> _devices;

        public BCDeviceController(IJwtBuilder jwtBuilder, IUserRepository userRepository, IEnumerable<IDevice> devices)
        {
            _jwtBuilder = jwtBuilder;
            _userRepository = userRepository;
            _devices = devices;
        }

        [HttpPost]
        public async Task<IActionResult> Add(BCDeviceRegistrationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var device = _devices.SingleOrDefault(d => d.Type == request.DeviceType);
                if (device == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_DEVICE_TYPE, request.DeviceType));
                var bearerToken = ExtractBearerToken();
                var user = await GetUser(bearerToken);
                var newDevice = device.Register(user, request);
                await _userRepository.SaveChanges(cancellationToken);
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = new JsonObject
                    {
                        ["id"] = newDevice.Id,
                    }.ToJsonString(),
                    ContentType = "application/json"
                };
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(string deviceType, double? lastView = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var device = _devices.SingleOrDefault(d => d.Type == deviceType);
                if (device == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_DEVICE_TYPE, deviceType));
                var bearerToken = ExtractBearerToken();
                var user = await GetUser(bearerToken);
                var result = await device.GetLastUnreadMessages(user, lastView, cancellationToken);
                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                return BuildError(ex);
            }
        }

        private async Task<User> GetUser(string bearerToken)
        {
            var result = _jwtBuilder.ReadSelfIssuedJsonWebToken(bearerToken);
            if (result.Error != null) throw new OAuthException(ErrorCodes.ACCESS_DENIED, result.Error);
            var user = await _userRepository.Query().Include(u => u.OAuthUserClaims).Include(u => u.Devices).FirstOrDefaultAsync(u => u.OAuthUserClaims.Any(c => c.Name == JwtRegisteredClaimNames.Sub && c.Value == result.Jwt.Subject));
            if (user == null) throw new OAuthException(ErrorCodes.UNKNOWN_USER, string.Format(ErrorMessages.UNKNOWN_USER, result.Jwt.Subject));
            return user;
        }
    }

    public class DeviceMessageResult
    {
        [BindProperty(Name = DeviceMessageResultParameters.DeviceId)]
        [JsonPropertyName(DeviceMessageResultParameters.DeviceId)]
        public string DeviceId { get; set; } = null!;
        [BindProperty(Name = DeviceMessageResultParameters.AuthReqId)]
        [JsonPropertyName(DeviceMessageResultParameters.AuthReqId)]
        public string AuthReqId { get; set; } = null!;
        [BindProperty(Name = DeviceMessageResultParameters.ClientId)]
        [JsonPropertyName(DeviceMessageResultParameters.ClientId)]
        public string? ClientId { get; set; } = null;
        [BindProperty(Name = DeviceMessageResultParameters.BindingMessage)]
        [JsonPropertyName(DeviceMessageResultParameters.BindingMessage)]
        public string? BindingMessage { get; set; } = null;
        [BindProperty(Name = DeviceMessageResultParameters.ReceptionDateTime)]
        [JsonPropertyName(DeviceMessageResultParameters.ReceptionDateTime)]
        public double ReceptionDateTime { get; set; }
        [BindProperty(Name = DeviceMessageResultParameters.Permissions)]
        [JsonPropertyName(DeviceMessageResultParameters.Permissions)]
        public IEnumerable<BCAuthorizePermission> Permissions { get; set; }
    }
}
