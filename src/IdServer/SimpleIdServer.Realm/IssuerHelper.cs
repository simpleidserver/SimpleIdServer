// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Realm.Middlewares;

namespace SimpleIdServer.Realm
{
    public static class IssuerHelper
    {
        public static string GetIssuer(string result) 
        {
            var realm = RealmContext.Instance().Realm;
            if (!string.IsNullOrWhiteSpace(realm))
            {
                if (!result.EndsWith("/"))
                    result += "/";

                result += realm;
            }

            return result;
        }
    }
}
