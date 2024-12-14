using FormBuilder.Models.Transformer;

namespace FormBuilder.Models.Rules;

public class LinkWorkflowMappingRule
{
    public string Source { get; set; }
    public string Target { get; set; }
    public ITransformerParameters Transformer { get; set; }
}