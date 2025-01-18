// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.UIs;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public interface ISidStepViewModel : IStepViewModel
{
    string ReturnUrl { get; set; }
    string Realm { get; set; }
}
