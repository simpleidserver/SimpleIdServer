using FormBuilder.Models.Url;
using FormBuilder.Urls;

namespace FormBuilder.Factories;

public interface IUrlEvaluatorFactory
{
    string Evaluate(ITargetUrl url);
}

public class UrlEvaluatorFactory : IUrlEvaluatorFactory
{
    private readonly IEnumerable<IUrlEvaluator> _urlEvaluators;

    public UrlEvaluatorFactory(IEnumerable<IUrlEvaluator> urlEvaluators)
    {
        _urlEvaluators = urlEvaluators;
    }

    public string Evaluate(ITargetUrl url)
    {
        var urlEvaluator = _urlEvaluators.Single(e => e.Type == url.Type);
        return urlEvaluator.Evaluate(url);
    }
}
