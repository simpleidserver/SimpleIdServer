// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Configuration
{
    public static class DisplayConditionEvaluator
    {
        public static bool IsLogicalOperationValid(Dictionary<string, string> values, string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return true;
            var conditions = condition.Split("&&").Select(c => c.Trim());
            return conditions.All(c => IsEqualityOperationValid(values, c));
        }

        public static bool IsEqualityOperationValid(Dictionary<string, string> values, string condition)
        {
            var splitted = condition.Split('=');
            var key = splitted[0];
            var value = splitted[1];
            return values.Any(kvp => kvp.Key == key && kvp.Value.Equals(value, System.StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
