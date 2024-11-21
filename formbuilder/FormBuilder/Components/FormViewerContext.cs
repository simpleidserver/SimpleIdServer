using FormBuilder.Models;

namespace FormBuilder.Components;

public class FormViewerContext
{
    private Action _droppedCallback;
    public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
    public IFormElementDefinition SelectedDefinition { get; private set; }
    public IFormElementRecord SelectedRecord { get; private set; }
    public SelectionTypes SelectionType { get; private set; }

    public void Set(IFormElementRecord record, Action droppedCallback)
    {
        _droppedCallback = droppedCallback;
        SelectedRecord = record;
        SelectedDefinition = null;
        SelectionType = SelectionTypes.RECORD;
    }

    public void Set(IFormElementDefinition def)
    {
        SelectedDefinition = def;
        SelectedRecord = null;
        SelectionType = SelectionTypes.DEFINITION;
    }

    public void DropElement()
    {
        SelectedDefinition = null;
        SelectedRecord = null;
        if (_droppedCallback != null) _droppedCallback();
    }
}

public enum SelectionTypes
{
    DEFINITION = 0,
    RECORD = 1
}