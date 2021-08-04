// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System;

namespace SimpleIdServer.OpenID.Domains
{
    public class AuthenticationSchemeProvider : ICloneable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string HandlerFullQualifiedName { get; set; }
        public string JsonConverter { get; set; }
        public string OptionsFullQualifiedName { get; set; }
        public string Options { get; set; }

        public JObject JsonOptions
        {
            get
            {
                return JObject.Parse(Options);
            }
        }

        public void Disable()
        {
            IsEnabled = false;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void UpdateOptions(JObject jObj)
        {
            Options = jObj.ToString();
            UpdateDateTime = DateTime.UtcNow;
        }

        public void Enable()
        {
            IsEnabled = true;
            UpdateDateTime = DateTime.UtcNow;
        }

        public object Clone()
        {
            return new AuthenticationSchemeProvider
            {
                Id = Id,
                Name = Name,
                CreateDateTime = CreateDateTime,
                UpdateDateTime = UpdateDateTime,
                DisplayName = DisplayName,
                HandlerFullQualifiedName = HandlerFullQualifiedName,
                IsEnabled = IsEnabled,
                Options = Options,
                OptionsFullQualifiedName = OptionsFullQualifiedName,
                JsonConverter = JsonConverter
            };
        }
    }
}
