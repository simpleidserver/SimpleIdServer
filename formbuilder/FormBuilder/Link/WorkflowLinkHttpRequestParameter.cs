using FormBuilder.Models.Rules;

namespace FormBuilder.Link;

public class WorkflowLinkHttpRequestParameter
{
    public string Target { get; set; }
    public bool IsAntiforgeryEnabled { get; set; }
    public HttpMethods Method { get; set; }
    public bool IsCustomParametersEnabled { get; set; }
    public List<MappingRule> Rules { get; set; } = new List<MappingRule>();
}

public enum HttpMethods
{
    GET = 0,
    POST = 1
}