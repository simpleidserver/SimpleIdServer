// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Realm.Middlewares
{
    public class RealmContext
    {
        private static RealmContext _instance;

        private RealmContext() { }

        public static RealmContext Instance()
        {
            if(_instance == null)
            {
                _instance = new RealmContext();
            }

            return _instance;
        }

        public string Realm { get; set; }
    }
}
