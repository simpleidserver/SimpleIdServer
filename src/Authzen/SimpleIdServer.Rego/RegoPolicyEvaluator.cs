namespace SimpleIdServer.Rego;

using System.Collections.Generic;

public class RegoPolicyEvaluator : RegoParserBaseVisitor<bool>
{
    private readonly Dictionary<string, object> _input;

    public RegoPolicyEvaluator(Dictionary<string, object> input)
    {
        _input = input;
    }

    public override bool VisitRoot(RegoParser.RootContext context)
    {
        foreach (var stmt in context.stmt())
        {
            if (stmt.regoRules() != null)
            {
                var rules = stmt.regoRules();
                if (rules.Default() != null && rules.Name().GetText() == "allow")
                {

                }
                else if (rules.ruleHead() != null && rules.ruleHead().Name().GetText() == "allow")
                {
                    foreach (var body in rules.ruleBody())
                    {
                        if (Visit(body)) return true;
                    }
                }
            }
        }

        return false;
    }

    public override bool VisitRuleBody(RegoParser.RuleBodyContext context)
    {
        if (context.nonEmptyBraceEnclosedBody() != null)
        {
            return Visit(context.nonEmptyBraceEnclosedBody());
        }

        return false;
    }

    public override bool VisitNonEmptyBraceEnclosedBody(RegoParser.NonEmptyBraceEnclosedBodyContext context)
    {
        return Visit(context.query());
    }

    public override bool VisitQuery(RegoParser.QueryContext context)
    {
        foreach (var literal in context.literal())
        {
            if (!Visit(literal)) return false;
        }
        return true;
    }

    public override bool VisitLiteral(RegoParser.LiteralContext context)
    {
        return Visit(context.literalExpr());
    }

    public override bool VisitExprTerm(RegoParser.ExprTermContext context)
    {
        if (context.ChildCount == 3)
        {
            var left = context.GetChild(0);
            var right = context.GetChild(2);
            var leftValue = left.GetText();
            var rightValue = right.GetText().Trim('"');
            if (leftValue == "input.user")
            {
                return _input.TryGetValue("user", out var userVal) && userVal?.ToString() == rightValue;
            }
        }
        
        return false;
    }
}
