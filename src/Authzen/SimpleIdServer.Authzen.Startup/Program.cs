using Antlr4.Runtime;
using System.Text.Json;

// See https://aka.ms/new-console-template for more information
var jsonText = File.ReadAllText("input.json");
var inputObj = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonText);
var regoText = File.ReadAllText("sample.rego");
var inputStream = new AntlrInputStream(regoText);
var lexer = new RegoLexer(inputStream);
var tokenStream = new CommonTokenStream(lexer);
var parser = new RegoParser(tokenStream);

// Le point d'entrée de la grammaire est probablement 'root' (voir RegoParser.g4)
var tree = parser.root();

// Évaluer la policy Rego sur l'input JSON
var evaluator = new RegoPolicyEvaluator(inputObj);
bool isAllowed = evaluator.Visit(tree);

Console.WriteLine($"allow = {isAllowed}");
