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
        if (record.Transformation == null) return;
        var inputData = context.GetCurrentStepInputData();
        var transformationResult = _transformationRuleEngineFactory.Transform(inputData, record.Transformation).Where(r => r != null);
        if (!transformationResult.Any()) return;
        record.Apply(transformationResult.First());
    }
}
