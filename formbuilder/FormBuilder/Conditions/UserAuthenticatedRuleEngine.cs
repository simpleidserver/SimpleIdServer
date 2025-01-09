using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;

namespace FormBuilder.Conditions;

public class UserAuthenticatedRuleEngine : GenericConditionRule<UserAuthenticatedParameter>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAuthenticatedRuleEngine(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override string Type => UserAuthenticatedParameter.TYPE;

    protected override bool EvaluateInternal(JsonObject input, UserAuthenticatedParameter parameter, IEnumerable<IConditionRuleEngine> conditionRuleEngines)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext.User.Identity.IsAuthenticated;
    }
}
