// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.UIs;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public interface IRegisterViewModel : IStepViewModel
{
    bool IsNotAllowed { get; set; }
    bool IsUpdated { get; set; }
    bool IsCreated { get; set; }
    string Amr { get; set; }
    string Realm { get; set; }
    List<string> Steps { get; set; }
    string? RedirectUrl { get; set; }
}