// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Extensions;
using System.Net;
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
        private readonly IOAuthUserRepository _oauthUserCommandRepository;

        public BCDeviceRegistrationHandler(
            IBCDeviceRegistrationValidator bcDeviceRegistrationValidator,
            IOAuthUserRepository oAuthUserCommandRepository)
        {
            _bcDeviceRegistrationValidator = bcDeviceRegistrationValidator;
            _oauthUserCommandRepository = oAuthUserCommandRepository;
        }

        public async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                var deviceRegistrationToken = context.Request.RequestData.GetDeviceRegistrationToken();
                var user = await _bcDeviceRegistrationValidator.Validate(context, cancellationToken);
                user.DeviceRegistrationToken = deviceRegistrationToken;
                await _oauthUserCommandRepository.Update(user, cancellationToken);
                await _oauthUserCommandRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch(OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}
