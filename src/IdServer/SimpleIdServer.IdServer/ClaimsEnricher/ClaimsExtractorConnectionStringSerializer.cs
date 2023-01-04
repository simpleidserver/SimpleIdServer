// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Linq;
using System.Reflection;

namespace SimpleIdServer.IdServer.ClaimsEnricher
{
    public class ClaimsExtractorConnectionStringSerializer
    {
        private const string SEPARATOR = ";";

        public T Deserialize<T>(string str)
        {
            var properties = str.Split(SEPARATOR).Select(r => r.Split('=')).ToDictionary(kvp => kvp[0], kvp => kvp[1]);
            var instance = Activator.CreateInstance<T>();
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach(var kvp in properties)
            {
                var prop = props.FirstOrDefault(p => p.Name == kvp.Key);
                if (prop == null) continue;
                prop.SetValue(str, kvp.Value);
            }

            return instance;
        }
    }
}
