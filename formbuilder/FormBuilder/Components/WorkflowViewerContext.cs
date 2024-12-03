using FormBuilder.Models;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components;

public class WorkflowViewerContext
{
    public event EventHandler<FormEltEventArgs> WorkflowLinkChanged;

    public void StartDragElt(ElementReference eltReference, IFormElementRecord record)
    {
        WorkflowLinkChanged(this, new FormEltEventArgs(eltReference, record));
    }
}

public class FormEltEventArgs : EventArgs
{
    public FormEltEventArgs(ElementReference eltReference, IFormElementRecord record)
    {
        EltReference = eltReference;
        Record = record;
    }

    public ElementReference EltReference { get; private set; }
    public IFormElementRecord Record { get; private set; }
}