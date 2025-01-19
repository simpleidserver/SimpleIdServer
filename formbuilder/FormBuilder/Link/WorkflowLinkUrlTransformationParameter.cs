using FormBuilder.Models.Transformer;

namespace FormBuilder.Link;

public class WorkflowLinkUrlTransformationParameter
{
    public string Url { get; set; }
    public string QueryParameterName { get; set; }
    public string JsonSource { get; set; }
    public List<ITransformerParameters> Transformers { get; set; }
}