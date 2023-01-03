// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.Store;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCDeviceRegistration
{
    public interface IBCDeviceRegistrationHandler
    {
        Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCDeviceRegistrationHandler : IBCDeviceRegistrationHandler
    {
        private readonly IBCDeviceRegistrationValidator _bcDeviceRegistrationValidator;
        private readonly IUserRepository _userRepository;

        public BCDeviceRegistrationHandler(
            IBCDeviceRegistrationValidator bcDeviceRegistrationValidator,
            IUserRepository userRepository)
        {
            _bcDeviceRegistrationValidator = bcDeviceRegistrationValidator;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                var deviceRegistrationToken = context.Request.RequestData.GetDeviceRegistrationToken();
                var user = await _bcDeviceRegistrationValidator.Validate(context, cancellationToken);
                user.DeviceRegistrationToken = deviceRegistrationToken;
                _userRepository.Update(user);
                await _userRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch(OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}
