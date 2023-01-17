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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.BCDevice
{
    public class BCDeviceController : Controller
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
        public async Task<IActionResult> Get()
        {
            var bearerToken = ExtractBearerToken();
            var user = await GetUser(bearerToken);
            // Get latest unread message.
            return null;
        }

        private ContentResult BuildError(OAuthException ex) => new ContentResult
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Content = new JsonObject
            {
                [ErrorResponseParameters.Error] = ex.Code,
                [ErrorResponseParameters.ErrorDescription] = ex.Message
            }.ToJsonString(),
            ContentType = "application/json"
        };

        private string ExtractBearerToken()
        {
            if(Request.Headers.ContainsKey(Constants.AuthorizationHeaderName))
            {
                foreach (var authorizationValue in Request.Headers[Constants.AuthorizationHeaderName])
                {
                    var at = authorizationValue.ExtractAuthorizationValue(new string[] { AutenticationSchemes.Bearer });
                    if (!string.IsNullOrWhiteSpace(at))
                    {
                        return at;
                    }
                }
            }

            throw new OAuthException(ErrorCodes.ACCESS_DENIED, ErrorMessages.MISSING_TOKEN);
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
        public string DeviceId { get; set; } = null!;
        public string AuthReqId { get; set; } = null!;
        public string? ClientId { get; set; } = null!;
        public string? BindingMessage { get; set; } = null;
        public DateTime ReceptionDateTime { get; set; }
        public IEnumerable<BCAuthorizePermission> Permissions { get; set; }
    }
}
