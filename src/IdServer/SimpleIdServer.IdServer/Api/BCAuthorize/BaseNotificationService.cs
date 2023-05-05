// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace SimpleIdServer.IdServer.Api.BCAuthorize
{
    public class BaseNotificationService
    {
        private readonly IDataProtector _dataProtector;
        private readonly UrlEncoder _urlEncoder;

        public BaseNotificationService(IDataProtectionProvider dataProtectionProvider, UrlEncoder urlEncoder)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _urlEncoder = urlEncoder;
        }

        protected string BuildUrl(IUrlHelper urlHelper, string issuer, BCNotificationMessage message)
        {
            var queries = message.Serialize(_urlEncoder);
            var queryCollection = new QueryBuilder(queries);
            var returnUrl = $"{HandlerContext.GetIssuer(issuer)}/{Constants.EndPoints.BCCallback}{queryCollection.ToQueryString()}";
            var url = urlHelper.Action("Index", "BackChannelConsents", new
            {
                returnUrl = _dataProtector.Protect(returnUrl)
            });
            return $"{issuer}{url}";
        }
    }
}
