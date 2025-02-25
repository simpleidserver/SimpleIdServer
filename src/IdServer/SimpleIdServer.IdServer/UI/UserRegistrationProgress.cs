// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.UI;

public class UserRegistrationProgress
{
    public string RegistrationProgressId { get; set; }
    public string WorkflowId { get; set; }
    public string Realm { get; set; }
    public User User { get; set; } = null;
    public string RedirectUrl { get; set; } = null;
    public bool UpdateOneCredential { get; set; }
}