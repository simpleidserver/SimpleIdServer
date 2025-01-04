using FormBuilder.Components;
using FormBuilder.Factories;
using FormBuilder.Models;

namespace FormBuilder;

public abstract class BaseFormFieldElementDefinition<T> : GenericFormElementDefinition<T> where T : BaseFormFieldRecord
{
    private readonly ITransformationRuleEngineFactory _transformationRuleEngineFactory;

    protected BaseFormFieldElementDefinition(ITransformationRuleEngineFactory transformationRuleEngineFactory)
    {
        _transformationRuleEngineFactory = transformationRuleEngineFactory;
    }

    protected override void ProtectedInit(T record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
        if (record.Transformations == null) return;
        var inputData = context.GetCurrentStepInputData();
        foreach(var transformation in record.Transformations)
            _transformationRuleEngineFactory.Apply(record, inputData, transformation);
    }
}
