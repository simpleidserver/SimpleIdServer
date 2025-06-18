// See https://aka.ms/new-console-template for more information

using SimpleIdServer.Authzen.Dtos;
using SimpleIdServer.Authzen.Rego;
using SimpleIdServer.Authzen.Rego.Compiler;
using SimpleIdServer.Authzen.Rego.Eval;
using Action = SimpleIdServer.Authzen.Dtos.Action;

static void DownloadOpa()
{
    var downloader = OpaDownloaderFactory.Build();
    downloader.DownloadOpaFile().Wait();
}

static void CompileOpa()
{
    var compilationResult = OpaCompilerFactory.Build().Compile().Result;
}

static void Eval()
{
    var evaluator = RegoEvaluatorFactory.Build();
    var request = new AccessEvaluationRequest
    {
        Action = new Action
        {
            Name = "read"
        },
        Subject = new Subject
        {
            Type = "user",
            Id = "admin",
            Properties = new Dictionary<string, object>
            {
                { "roles", new string[]  { "admin" } }
            }
        },
        Resource = new Resource
        {
            Id = "inv-001",
            Type = "users"
        }
    };
    var evaluationResult = evaluator.Evaluate(request, CancellationToken.None).Result;
    Console.WriteLine($"Policy evaluation result {evaluationResult}");
    // avoir une convention explicite pour mes règles rego
    // policies/{resource_type}/{action}.rego
    // exemple de fichier JSON 
    // Exemple de chemin : path = "/v1/data/policy/{input.resource.type}/{input.action}/allow"
    /*
    {
    "subject": {
        "sub": "user123",
        "roles": ["admin"]
    },
    "resource": {
        "type": "invoice",
        "id": "inv-001",
        "owner": "user456"
    },
    "action": "delete",
    "context": {
        "ip": "192.168.1.10",
        "time": "2025-06-01T12:00:00Z"
    }
    }
    */
}

DownloadOpa();
CompileOpa();
Eval();
