using FormBuilder.Models.Url;

namespace FormBuilder.Urls;

public class DirectTargetUrlEvaluator : BaseGenericUrlEvaluator<DirectTargetUrl>
{
    public override string Type => DirectTargetUrl.TYPE;

    protected override string InternalEvaluate(DirectTargetUrl targetUrl)
        => $"{targetUrl.Url}?{string.Join("&", targetUrl.Parameters.Select(p => $"{p.Key}={p.Value}"))}";
}
