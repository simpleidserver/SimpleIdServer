using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using System.Collections.ObjectModel;

namespace FormBuilder.Link;

public class WorkflowLinkHttpRequestParameter
{
    public string Target { get; set; }
    public List<ITransformerParameters> Transformers { get; set; }
    public bool IsAntiforgeryEnabled { get; set; }
    public HttpMethods Method { get; set; }
    public bool IsCustomParametersEnabled { get; set; }
    public ObservableCollection<MappingRule> Rules { get; set; } = new ObservableCollection<MappingRule>();
}

public enum HttpMethods
{
    GET = 0,
    POST = 1
}