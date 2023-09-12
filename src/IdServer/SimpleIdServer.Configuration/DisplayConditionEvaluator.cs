// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Configuration
{
    public static class DisplayConditionEvaluator
    {
        public static bool IsValid(Dictionary<string, string> values, string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return true;
            var splitted = condition.Split('=');
            var key = splitted[0];
            var value = splitted[1];
            return values.Any(kvp => kvp.Key == key && kvp.Value == value);
        }
    }
}
