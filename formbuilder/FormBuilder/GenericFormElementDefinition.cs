using FormBuilder.Components;
using FormBuilder.Models;

namespace FormBuilder;

public abstract class GenericFormElementDefinition<T> : IFormElementDefinition where T : IFormElementRecord
{
    public abstract Type UiElt { get; }
    public Type RecordType => typeof(T);
    public abstract string Type { get; }
    public abstract string Icon { get; }
    public abstract ElementDefinitionCategories Category { get; }

    public void Init(IFormElementRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
        => ProtectedInit((T)record, context, definitions);

    protected abstract void ProtectedInit(T record, WorkflowContext context, List<IFormElementDefinition> definitions);
}
