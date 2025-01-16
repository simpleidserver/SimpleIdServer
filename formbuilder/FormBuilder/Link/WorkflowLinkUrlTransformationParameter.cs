using FormBuilder.Transformers;

namespace FormBuilder.Link;

public class WorkflowLinkUrlTransformationParameter
{
    public string Url { get; set; }
    public string QueryParameterName { get; set; }
    public string JsonSource { get; set; }
    public RegexTransformerParameters Transformer { get; set; }
}