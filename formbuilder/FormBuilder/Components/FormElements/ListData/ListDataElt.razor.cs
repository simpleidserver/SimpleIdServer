﻿using FormBuilder.Components.Drag;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.ListData
{
    public partial class ListDataElt : IGenericFormElement<ListDataRecord>
    {
        private RenderFragment? CustomRender { get; set; }
        [Inject] private IRenderFormElementsHelper renderFormsElementsHelper { get; set; }
        [Parameter] public FormViewerContext Context { get; set; }
        [Parameter] public ListDataRecord Value { get; set; }
        [Parameter] public bool IsEditModeEnabled { get; set; }
        [Parameter] public ParentEltContext ParentContext { get; set; }
        [Parameter] public WorkflowViewerContext WorkflowContext { get; set; }
        [Parameter] public bool IsInteractableElementEnabled { get; set; }
        [Parameter] public WorkflowExecutionContext WorkflowExecutionContext { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Value != null)
            {
                CustomRender = CreateComponent();
            }
        }

        private RenderFragment CreateComponent() => builder =>
        {
            renderFormsElementsHelper.Render(builder, Value.Elements, Context, IsEditModeEnabled, WorkflowContext, IsInteractableElementEnabled, WorkflowExecutionContext);
        };
    }
}
