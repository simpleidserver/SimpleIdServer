// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;

namespace SimpleIdServer.OAuth.Api.Authorization
{
    public class RedirectActionAuthorizationResponse : AuthorizationResponse
    {
        public RedirectActionAuthorizationResponse(string action, string controllerName, JObject queryParameters) : base(AuthorizationResponseTypes.RedirectAction)
        {
            Action = action;
            ControllerName = controllerName;
            QueryParameters = queryParameters;
        }

        public RedirectActionAuthorizationResponse(string action, string controllerName, JObject queryParameters, string area) : this(action, controllerName, queryParameters)
        {
            Area = area;
        }

        public string Action { get; private set; }
        public string ControllerName { get; private set; }
        public JObject QueryParameters { get; private set; }
        public string Area { get; private set; }
    }
}
