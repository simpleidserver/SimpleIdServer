// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class ExternalAuthLoginViewModel : ISidStepViewModel
{
    public string ReturnUrl { get; set; }
    public string Realm { get; set; }
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }
}
