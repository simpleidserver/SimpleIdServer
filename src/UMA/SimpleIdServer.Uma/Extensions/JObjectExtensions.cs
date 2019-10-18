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
            return jObj.GetTranslationsFromRegisterRequest(UMAResourceNames.Description);
        }

        public static string GetUMAIconURIFromRequest(this JObject jObj)
        {
            return jObj.GetStr(UMAResourceNames.IconUri);
        }

        public static Dictionary<string, string> GetUMANameFromRequest(this JObject jObj)
        {
            return jObj.GetTranslationsFromRegisterRequest(UMAResourceNames.Name);
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
            return jObj.GetStr(TokenRequestParameters.ClientSecret);
        }

        #endregion
    }
}
