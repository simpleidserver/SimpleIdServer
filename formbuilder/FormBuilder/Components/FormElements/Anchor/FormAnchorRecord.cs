using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Anchor
{
    public class FormAnchorRecord : BaseFormFieldRecord
    {
        public bool ActAsButton { get; set; } = false;

        public override void ExtractJson(JsonObject json)
        {
        }
    }
}
