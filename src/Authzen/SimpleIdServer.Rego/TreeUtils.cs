namespace SimpleIdServer.Rego;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Text;

public static class TreeUtils
{
    public static string ToPrettyTree(IParseTree tree, Parser parser)
    {
        var builder = new StringBuilder();
        BuildTree(tree, builder, 0, parser);
        return builder.ToString();
    }

    private static void BuildTree(IParseTree tree, StringBuilder builder, int indent, Parser parser)
    {
        // Ajouter l'indentation
        builder.Append("  ".Repeat(indent));

        // Obtenir le nom du nœud
        var ruleName = string.Empty;
        if (tree is ParserRuleContext context)
        {
            ruleName = parser.RuleNames[context.RuleIndex];
        }

        // Ajouter le nom du nœud et son texte si c'est une feuille
        if (tree.ChildCount == 0)
        {
            builder.AppendLine($"{ruleName} '{tree.GetText()}'");
        }
        else
        {
            builder.AppendLine(ruleName);
            // Récursivement construire l'arbre pour chaque enfant
            for (int i = 0; i < tree.ChildCount; i++)
            {
                BuildTree(tree.GetChild(i), builder, indent + 1, parser);
            }
        }
    }
}

public static class StringExtensions
{
    public static string Repeat(this string str, int count)
    {
        var builder = new StringBuilder(str.Length * count);
        for (int i = 0; i < count; i++)
            builder.Append(str);
        return builder.ToString();
    }
}
