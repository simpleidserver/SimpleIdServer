// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Saml.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] arr, T obj)
        {
            var items = new List<T>();
            if (arr != null)
            {
                items = arr.ToList();
            }

            items.Add(obj);
            return items.ToArray();
        }
    }
}
