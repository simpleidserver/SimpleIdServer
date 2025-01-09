// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.UIs;

namespace SimpleIdServer.IdServer.Pwd.UI.ViewModels;

public class ResetPasswordIndexViewModel : IStepViewModel
{
    public string ReturnUrl { get; set; } = null!;
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }
}
