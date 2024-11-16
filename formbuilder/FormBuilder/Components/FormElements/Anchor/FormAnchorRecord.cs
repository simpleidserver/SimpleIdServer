using FormBuilder.Models;
using FormBuilder.Models.Url;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Anchor
{
    public class FormAnchorRecord : BaseFormFieldRecord
    {
        public ITargetUrl Url { get; set; }
        public bool ActAsButton { get; set; } = false;

        public override void ExtractJson(JsonObject json)
        {
        }
    }
}
