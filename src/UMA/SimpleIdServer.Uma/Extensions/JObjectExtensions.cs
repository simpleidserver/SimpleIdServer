// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.Uma.DTOs;
using System.Collections.Generic;

namespace SimpleIdServer.Uma.Extensions
{
    public static class JObjectExtensions
    {
        #region UMA resource

        #endregion

        #region Token request

        #endregion

        #region Ticket request

        public static string GetResourceId(this JObject jObj)
        {
            return jObj.GetStr(UMAPermissionNames.ResourceId);
        }

        public static IEnumerable<string> GetResourceScopes(this JObject jObj)
        {
            return jObj.GetArray(UMAPermissionNames.ResourceScopes);
        }

        #endregion
    }
}
