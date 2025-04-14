// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.Drag;
using FormBuilder.Helpers;
using FormBuilder.Services;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Back;

public partial class BackButton : IGenericFormElement<BackButtonRecord>
{
    [Parameter] public BackButtonRecord Value { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Inject] private INavigationHistoryService navigationHistoryService { get; set; }
    [Inject] private IHtmlClassResolver htmlClassResolver { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    private async Task HandleBack()
    {
        await navigationHistoryService.Back(Context);
    }

    public string BtnClass
    {
        get
        {
            return htmlClassResolver.Resolve(Value, BackButtonElementNames.Btn, Context);
        }
    }
}
