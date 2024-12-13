using FormBuilder.Models.Url;

namespace FormBuilder.Urls;

public abstract class BaseGenericUrlEvaluator<T> : IUrlEvaluator where T : ITargetUrl
{
    public abstract string Type { get; }

    public string Evaluate(ITargetUrl targetUrl) => InternalEvaluate((T)targetUrl);

    protected abstract string InternalEvaluate(T  targetUrl);
}
