using FormBuilder.Models;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FormBuilder.Components.FormElements.Button;

public class FormButtonRecord : BaseFormFieldRecord
{
    public override string Type => FormButtonDefinition.TYPE;
    private bool _isSubmitting;

    [JsonIgnore]
    public bool IsSubmitting
    {
        get
        {
            return _isSubmitting;
        }
        set
        {
            _isSubmitting = value;
            IsSubmittingChanged(this, new EventArgs());
        }
    }
    [JsonIgnore]
    public EventHandler<EventArgs> IsSubmittingChanged { get; set; }

    public override void ExtractJson(JsonObject json) { }

    public override void Apply(JsonNode node)
    {

    }
}