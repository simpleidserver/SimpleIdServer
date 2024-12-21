﻿using FormBuilder.Components.Drag;
using FormBuilder.Helpers;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace FormBuilder.Components.FormElements.ListData
{
    public partial class ListDataElt : IGenericFormElement<ListDataRecord>
    {
        private RenderFragment? CustomRender { get; set; }
        [Inject] private IRenderFormElementsHelper renderFormsElementsHelper { get; set; }
        [Parameter] public WorkflowContext Context { get; set; }
        [Parameter] public ListDataRecord Value { get; set; }
        [Parameter] public bool IsEditModeEnabled { get; set; }
        [Parameter] public ParentEltContext ParentContext { get; set; }
        [Parameter] public bool IsInteractableElementEnabled { get; set; }

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
            foreach(var record in Value.Elements)
            {
                var col = new ObservableCollection<IFormElementRecord>
                {
                    record
                };
                renderFormsElementsHelper.Render(builder, col, Context, IsEditModeEnabled, IsInteractableElementEnabled);
            }
        };
    }
}