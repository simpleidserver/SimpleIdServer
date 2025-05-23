namespace SimpleIdServer.Rego;

using System.Collections.Generic;
using System.Text.Json;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

public class RegoEvaluator
{
    public static string Evaluate(string rego, string input)
    {
        var inputObj = JsonSerializer.Deserialize<Dictionary<string, object>>(input);
        return Evaluate(rego, inputObj);
    }

    public static string Evaluate(string rego, Dictionary<string, object> inputObj)
    {
        var root = ResolveRoot(rego);
        var evaluator = new RegoPolicyEvaluator(inputObj);
        var isAllowed = evaluator.Visit(root);
        return isAllowed.ToString();
    }

    public static string ToPrettyTree(string rego)
    {
        var parser = ResolveParser(rego);
        return TreeUtils.ToPrettyTree(parser.root(), parser);
    }

    private static IParseTree ResolveRoot(string rego)
    {
        var parser = ResolveParser(rego);
        return parser.root();
    }

    private static RegoParser ResolveParser(string rego)
    {
        var inputStream = new AntlrInputStream(rego);
        var lexer = new RegoLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new RegoParser(tokenStream);
        return parser;
    }
}