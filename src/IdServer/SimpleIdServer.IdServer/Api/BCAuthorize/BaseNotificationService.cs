// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.IdServer.Api.BCAuthorize
{
    public class BaseNotificationService
    {
        private readonly IDataProtector _dataProtector;

        public BaseNotificationService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
        }

        protected string BuildUrl(IUrlHelper urlHelper, string issuer, BCNotificationMessage message)
        {
            var queries = message.Serialize();
            var queryCollection = new QueryBuilder(queries);
            var returnUrl = $"{HandlerContext.GetIssuer(issuer)}/{Constants.EndPoints.BCCallback}{queryCollection.ToQueryString()}";
            return $"{issuer}{urlHelper.Action("Index", "BackChannelConsents", new
            {
                returnUrl = _dataProtector.Protect(returnUrl)
            })}";
        }
    }
}
