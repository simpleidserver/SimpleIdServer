using SimpleIdServer.Rego;

static void DisplayTree(string rego)
{
    var prettyTree = RegoEvaluator.ToPrettyTree(rego);
    Console.WriteLine("Pretty Tree:");
    Console.WriteLine(prettyTree);
}

static void RegoAllowPolicy()
{
    var input = File.ReadAllText("input.json");
    var rego = File.ReadAllText("sample.rego");
    var evaluationReuslt = RegoEvaluator.Evaluate(input, rego);
    var prettyTree = RegoEvaluator.ToPrettyTree(rego);
    Console.WriteLine($"allow = {evaluationReuslt}");
    DisplayTree(rego);
}

static void RegoSimpleVariable()
{
    var rego = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "basics", "simple-variable.rego"));
    DisplayTree(rego);
}

RegoSimpleVariable();