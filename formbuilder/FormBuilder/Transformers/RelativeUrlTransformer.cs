using FormBuilder.Helpers;
using FormBuilder.Models.Transformer;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;

namespace FormBuilder.Transformers;

public class RelativeUrlTransformerParameters : ITransformerParameters
{
    public static string TYPE => "RelativeUrl";
    public string Type => TYPE;
}

public class RelativeUrlTransformer : GenericTransformer<RelativeUrlTransformerParameters>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpRequestState _httpRequestState;

    public RelativeUrlTransformer(IHttpContextAccessor httpContextAccessor, IHttpRequestState httpRequestState)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpRequestState = httpRequestState;
    }

    public override string Type => RelativeUrlTransformerParameters.TYPE;

    public override ITransformerParameters CreateEmptyInstance() => new RelativeUrlTransformerParameters();

    internal override void InternalBuild(RelativeUrlTransformerParameters parameters, RenderTreeBuilder builder)
    {
        
    }

    internal override object InternalTransform(string value, RelativeUrlTransformerParameters parameters, JsonNode data)
    {
        string request;
        if (_httpContextAccessor?.HttpContext != null)
        {
            request = _httpContextAccessor.HttpContext.Request.GetAbsoluteUriWithVirtualPath();
        }
        else
        {
            request = _httpRequestState.GetAbsoluteUriWithVirtualPath();
        }

        return $"{request}{value}";
    }
}
