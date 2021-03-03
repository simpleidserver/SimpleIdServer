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

        public static IEnumerable<string> GetUMAScopesFromRequest(this JObject jObj)
        {
            return jObj.GetArray(UMAResourceNames.ResourceScopes);
        }

        public static Dictionary<string, string> GetUMADescriptionFromRequest(this JObject jObj)
        {
            return jObj.GetTranslations(UMAResourceNames.Description);
        }

        public static string GetUMAIconURIFromRequest(this JObject jObj)
        {
            return jObj.GetStr(UMAResourceNames.IconUri);
        }

        public static Dictionary<string, string> GetUMANameFromRequest(this JObject jObj)
        {
            return jObj.GetTranslations(UMAResourceNames.Name);
        }

        public static string GetUMATypeFromRequest(this JObject jObj)
        {
            return jObj.GetStr(UMAResourceNames.Type);
        }

        public static string GetUMASubjectFromRequest(this JObject jObj)
        {
            return jObj.GetStr(UMAResourceNames.Subject);
        }

        #endregion

        #region Token request

        public static string GetTicket(this JObject jObj)
        {
            return jObj.GetStr(UMATokenRequestParameters.Ticket);
        }

        public static string GetClaimToken(this JObject jObj)
        {
            return jObj.GetStr(UMATokenRequestParameters.ClaimToken);
        }

        public static string GetClaimTokenFormat(this JObject jObj)
        {
            return jObj.GetStr(UMATokenRequestParameters.ClaimTokenFormat);
        }

        public static string GetPct(this JObject jObj)
        {
            return jObj.GetStr(UMATokenRequestParameters.Pct);
        }

        public static string GetRpt(this JObject jObj)
        {
            return jObj.GetStr(UMATokenRequestParameters.Rpt);
        }

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
