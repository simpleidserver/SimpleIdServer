// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains;

public class RegistrationWorkflow
{
    public string Id {  get; set; }
    public string Name { get; set; }
    public string RealmName { get; set; }
    public DateTime CreateDateTime {  get; set; }
    public DateTime UpdateDateTime { get; set; }
    public List<string> Steps {  get; set; } = new List<string>();
    public bool IsDefault { get; set; }
    public Realm Realm { get; set; }
}
