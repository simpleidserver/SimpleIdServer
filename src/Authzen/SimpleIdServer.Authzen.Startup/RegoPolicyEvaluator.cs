using System.Collections.Generic;
using Antlr4.Runtime.Misc;

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
            var result = Visit(stmt);
            if (result) return true;
        }

        return false;
    }

    // Visite une déclaration de règle
    public override bool VisitRegoRules(RegoParser.RegoRulesContext context)
    {
        if (context.GetText().Contains("allow"))
        {
            foreach (var child in context.children)
            {
                var result = Visit(child);
                if (result) return true;
            }
        }

        return false;
    }

    public override bool VisitRuleHead(RegoParser.RuleHeadContext context)
    {
        if (context.Name().GetText() == "allow")
        {
            foreach (var child in context.children)
            {
                // Cherche un nœud qui contient une égalité (EqOper)
                if (child is RegoParser.RuleBodyContext ruleBody)
                {
                    var result = Visit(ruleBody);
                    if (result) return true;
                }
                else if (child is RegoParser.NonEmptyBraceEnclosedBodyContext nonEmptyBody)
                {
                    var result = Visit(nonEmptyBody);
                    if (result) return true;
                }
            }
        }

        return false;
    }

    public override bool VisitLiteralExpr(RegoParser.LiteralExprContext context)
    {
        // Rego: literalExpr : exprTerm (EqOper exprTerm)* ;
        var exprTerms = context.exprTerm();
        var eqOpers = context.EqOper();
        if (exprTerms.Length == 2 && eqOpers.Length == 1)
        {
            var left = exprTerms[0].GetText();
            var right = exprTerms[1].GetText().Trim('"');
            if (eqOpers[0].GetText() == "==" && left.StartsWith("input.user") && _input.TryGetValue("user", out var user))
            {
                return user?.ToString() == right;
            }
        }
        return false;
    }
}
