
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.StackLayout;

public class FormStackLayoutDefinition : GenericFormElementDefinition<FormStackLayoutRecord>
{
    public static string TYPE = "Stacklayout";
    public override Type UiElt => typeof(FormStackLayout);
    public override string Type => TYPE;
    public override string Icon => "view_agenda";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.LAYOUT;

    protected override void ProtectedInit(FormStackLayoutRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
        if (record.Elements == null) return;
        foreach (var elt in record.Elements)
        {
            var definition = definitions.Single(d => d.Type == elt.Type);
            definition.Init(elt, context, definitions);
        }
    }
}
