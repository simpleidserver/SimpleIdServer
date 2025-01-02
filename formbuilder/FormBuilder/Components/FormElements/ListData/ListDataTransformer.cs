using FormBuilder.Components.FormElements.StackLayout;
using Json.Path;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.ListData;

public class ListDataTransformer : GenericFormElementTransformer<ListDataRecord>
{
    public override string Name => ListDataDefinition.TYPE;

    protected override JsonNode ProtectedTransform(JsonObject input, ListDataRecord record)
    {
        var path = JsonPath.Parse(record.RepetitionRule.Path);
        var pathResult = path.Evaluate(input);
        var nodes = pathResult.Matches.Select(m => m.Value);
        return nodes.FirstOrDefault()?.AsObject();
    }
}
