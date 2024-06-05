// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Authorization
{
    public class RedirectActionAuthorizationResponse : AuthorizationResponse
    {
        public RedirectActionAuthorizationResponse(string action, string controllerName, JsonObject queryParameters) : base(AuthorizationResponseTypes.RedirectAction)
        {
            Action = action;
            ControllerName = controllerName;
            QueryParameters = queryParameters;
        }

        public RedirectActionAuthorizationResponse(string action, string controllerName, JsonObject queryParameters, string area, bool disconnect = false, List<string> cookiesToRemove = null, AmrAuthInfo amrAuthInfo = null) : this(action, controllerName, queryParameters)
        {
            Area = area;
            Disconnect = disconnect;
            CookiesToRemove = cookiesToRemove;
            AmrAuthInfo = amrAuthInfo;
        }

        public string Action { get; private set; }
        public string ControllerName { get; private set; }
        public JsonObject QueryParameters { get; private set; }
        public string Area { get; private set; }
        public bool Disconnect { get; private set; }
        public List<string> CookiesToRemove { get; private set; }
        public AmrAuthInfo AmrAuthInfo { get; private set; }
    }
}
