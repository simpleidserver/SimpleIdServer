// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.UIs;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class SidWorkflowViewModel : WorkflowViewModel, ILayoutViewModel
{
    public List<Language> Languages { get; set; }
    public List<string> SupportedLanguageCodes => Languages.Select(l => l.Code).ToList();
}
