// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleIdServer.Scim.Provisioning.Helpers
{
    public static class TemplateParser
    {
        public static string ParseMessage(string template, JObject jObj)
        {
            var regularExpression = new Regex(@"\{{([a-zA-Z]|_|[0-9]|\[|\]|\.|\?)*\}}");
            var result = regularExpression.Replace(template, (m) =>
            {
                if (string.IsNullOrWhiteSpace(m.Value))
                {
                    return string.Empty;
                }

                var value = m.Value.Replace("{{", "");
                value = value.Replace("}}", "");
                var splitted = value.Split("??").Select(v => v.Replace(" ", ""));
                string otherValue = string.Empty;
                value = splitted.First();
                if (splitted.Count() == 2)
                {
                    otherValue = splitted.Last();
                }

                var token = jObj.SelectToken(value);
                if (token == null)
                {
                    if (!string.IsNullOrWhiteSpace(otherValue))
                    {
                        token = jObj.SelectToken(otherValue);
                        if (token != null)
                        {
                            return token.ToString();
                        }
                    }

                    return string.Empty;
                }

                return token.ToString();
            });
            return result;
        }
    }
}
