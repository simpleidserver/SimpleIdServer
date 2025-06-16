using FormBuilder.Factories;

namespace FormBuilder.Components.FormElements.Row
{
    public class RowLayoutDefinition : GenericFormElementDefinition<RowLayoutRecord>
    {
        private readonly ITransformationRuleEngineFactory _transformationRuleEngineFactory;

        public RowLayoutDefinition(ITransformationRuleEngineFactory transformationRuleEngineFactory)
        {
            
        }


        public static string TYPE = "RowLayout";

        public override Type UiElt => typeof(RowLayout);

        public override string Type => TYPE;

        public override string Icon => "flex_no_wrap";

        public override ElementDefinitionCategories Category => ElementDefinitionCategories.LAYOUT;

        protected override void ProtectedInit(RowLayoutRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
        {
            if (record.Elements != null)
            {
                foreach (var elt in record.Elements.Where(e => e != null))
                {
                    var definition = definitions.Single(d => d.Type == elt.Type);
                    definition.Init(elt, context, definitions);
                }
            }

            if (record.Transformations == null) return;
            var inputData = context.GetCurrentStepInputData();
            foreach (var transformation in record.Transformations)
                _transformationRuleEngineFactory.Apply(record, inputData, transformation);
        }
    }
}
