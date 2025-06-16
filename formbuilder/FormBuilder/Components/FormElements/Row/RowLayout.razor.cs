using FormBuilder.Components.Drag;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace FormBuilder.Components.FormElements.Row
{
    public partial class RowLayout : IGenericFormElement<RowLayoutRecord>
    {
        [Parameter]
        public RowLayoutRecord Value 
        { 
            get; set; 
        }

        [Parameter]
        public ParentEltContext ParentContext 
        { 
            get; set; 
        }

        [Parameter]
        public WorkflowContext Context 
        { 
            get; set; 
        }

        [Parameter]
        public bool IsEditModeEnabled 
        { 
            get; set; 
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Value != null)
            {
                if (Value.Elements == null)
                {
                    Value.Elements = new ObservableCollection<IFormElementRecord>();
                }
            }
        }

        private int ComputeSize()
        {
            return 12 / Value.NbColumns;
        }
    }
}
