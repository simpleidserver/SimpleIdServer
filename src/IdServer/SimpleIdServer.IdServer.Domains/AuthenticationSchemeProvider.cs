// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Domains
{
    public class AuthenticationSchemeProvider
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public bool IsEnabled { get; set; } = false;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string? HandlerFullQualifiedName { get; set; } = null;
        public string? OptionsFullQualifiedName { get; set; } = null;
        public string? SerializedOptions { get; set; } = null;

        public JsonObject? JsonOptions
        {
            get
            {
                return JsonSerializer.SerializeToNode(SerializedOptions)?.AsObject();
            }
        }

        public void Disable()
        {
            IsEnabled = false;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void UpdateOptions(JsonObject jObj)
        {
            SerializedOptions = jObj.ToString();
            UpdateDateTime = DateTime.UtcNow;
        }

        public void Enable()
        {
            IsEnabled = true;
            UpdateDateTime = DateTime.UtcNow;
        }
    }
}
