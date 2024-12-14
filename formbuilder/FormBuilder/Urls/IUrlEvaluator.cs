using FormBuilder.Models.Url;

namespace FormBuilder.Urls;

public interface IUrlEvaluator
{
    string Type { get; }
    string Evaluate(ITargetUrl targetUrl);
}
