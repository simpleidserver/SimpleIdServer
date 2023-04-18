// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Domains
{
    public class UserWalletCredential
    {
        public string Id { get; set; } = null!;
        public string SerializedValue { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public JsonObject Value
        {
            get
            {
                return JsonObject.Parse(SerializedValue).AsObject();
            }
            set
            {
                SerializedValue = JsonSerializer.Serialize(Value);
            }
        }

        public User User { get; set; } = null!;
    }
}
