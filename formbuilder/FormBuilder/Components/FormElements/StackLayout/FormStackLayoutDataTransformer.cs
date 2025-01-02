using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.StackLayout;

public class FormStackLayoutDataTransformer : GenericFormElementTransformer<FormStackLayoutRecord>
{
    public override string Name => FormStackLayoutDefinition.TYPE;

    protected override JsonNode ProtectedTransform(JsonObject input, FormStackLayoutRecord record)
    {
        var json = new JsonObject();
        foreach(var elt in record.Elements.Select(e => e as BaseFormFieldRecord).Where(e => e != null).Cast<BaseFormFieldRecord>())
        {
            if (elt.Name != null && input.ContainsKey(elt.Name))
                json.Add(elt.Name, input[elt.Name].ToString());
        }

        return json;
    }
}
