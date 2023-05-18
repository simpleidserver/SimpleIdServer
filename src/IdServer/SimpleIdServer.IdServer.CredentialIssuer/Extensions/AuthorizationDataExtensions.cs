// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Domains
{
    public static class AuthorizationDataExtensions
    {
        public static string GetFormat(this AuthorizationData authorizationData)
        {
            if (!authorizationData.AdditionalData.ContainsKey(AuthorizationDataParameters.Format)) return null;
            return authorizationData.AdditionalData[AuthorizationDataParameters.Format];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationData"></param>
        /// <returns></returns>
        public static IEnumerable<string>? GetTypes(this AuthorizationData authorizationData)
        {
            if (!authorizationData.AdditionalData.ContainsKey(AuthorizationDataParameters.Types)) return null;
            var str = authorizationData.AdditionalData[AuthorizationDataParameters.Types];
            if (string.IsNullOrWhiteSpace(str)) return null;
            try
            {
                return JsonArray.Parse(str).AsArray().Select(s => s.GetValue<string>());
            }
            catch
            {
                return null;
            }
        }
    }
}
