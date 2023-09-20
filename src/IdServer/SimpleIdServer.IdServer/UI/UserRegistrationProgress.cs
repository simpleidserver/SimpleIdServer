// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.UI;

public class UserRegistrationProgress
{
    public string RegistrationProgressId { get; set; }
    public string WorkflowName { get; set; }
    public List<string> Steps { get; set; }
    public string Amr { get; set; }
    public string Realm { get; set; }
    public User User { get; set; } = null;

    public void NextAmr()
    {
        var lastStep = Steps.Last();
        if (Amr == lastStep) return;
        var nextAmr = Steps.ElementAt(Steps.FindIndex(p => p == Amr) + 1);
        Amr = nextAmr;
    }
}
