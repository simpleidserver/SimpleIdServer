// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using SimpleIdServer.IdServer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.BCAuthorize
{
    public interface IBCNotificationService
    {
        Task Notify(HandlerContext handlerContext, BCNotificationMessage message, CancellationToken cancellationToken);
    }

    public class BCNotificationMessage
    {
        public string AuthReqId { get; set; }
        public string BindingMessage { get; set; }
        public string ClientId { get; set; }
        public IEnumerable<string> Scopes { get; set; } = new List<string>();
        public IEnumerable<string> AcrLst { get; set; } = new List<string>();
        public string Amr { get; set; }

        public List<KeyValuePair<string, string>> Serialize()
        {
            var result = new List<KeyValuePair<string, string>>();
            result.Add(new KeyValuePair<string, string>(BCAuthenticationResponseParameters.AuthReqId, AuthReqId));
            result.Add(new KeyValuePair<string, string>(AuthorizationRequestParameters.ClientId, ClientId));
            result.Add(new KeyValuePair<string, string>("amr", Amr));
            if (!string.IsNullOrWhiteSpace(BindingMessage))
                result.Add(new KeyValuePair<string, string>(BCAuthenticationRequestParameters.BindingMessage, BindingMessage));
            if (Scopes.Any())
                result.Add(new KeyValuePair<string, string>(AuthorizationRequestParameters.Scope, string.Join(" ", Scopes)));
            if (AcrLst.Any())
                result.Add(new KeyValuePair<string, string>(AuthorizationRequestParameters.AcrValue, string.Join(" ", AcrLst)));
            return result;
        }
    }

    public class BCConsoleNotificationService : BaseNotificationService, IBCNotificationService
    {
        public BCConsoleNotificationService(IDataProtectionProvider dataProtectionProvider) : base(dataProtectionProvider) {  }

        public Task Notify(HandlerContext handlerContext, BCNotificationMessage message, CancellationToken cancellationToken)
        {
            var before = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"The Back Channel redirection URL is : {BuildUrl(handlerContext.UrlHelper, handlerContext.GetIssuer(), message)}");
            Console.ForegroundColor = before;
            return Task.CompletedTask;
        }
    }
}
