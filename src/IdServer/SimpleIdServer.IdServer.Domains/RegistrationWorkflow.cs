// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class RegistrationWorkflow
{
    [JsonPropertyName("id")]
    public string Id {  get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonIgnore]
    public string RealmName { get; set; }
    [JsonIgnore]
    public DateTime CreateDateTime {  get; set; }
    [JsonIgnore]
    public DateTime UpdateDateTime { get; set; }
    [JsonIgnore]
    public List<string> Steps {  get; set; } = new List<string>();
    [JsonIgnore]
    public bool IsDefault { get; set; }
    [JsonIgnore]
    public Realm Realm { get; set; }
    [JsonIgnore]
    public ICollection<AuthenticationContextClassReference> Acrs { get; set; }
}
