// See https://aka.ms/new-console-template for more information

using SimpleIdServer.Authzen.Rego;
using SimpleIdServer.Authzen.Rego.Compiler;
using SimpleIdServer.Authzen.Rego.Discover;

static void DownloadOpa()
{
    var downloader = OpaDownloaderFactory.Build();
    downloader.DownloadOpaFile().Wait();
}

static void DiscoverPolicies()
{
    var resolver = RegoPoliciesResolverFactory.Build();
    var tt = resolver.Discover().Result;
}

static void CompileOpa()
{
    var compilationResult = OpaCompilerFactory.Build().Compile().Result;
}

static void Eval()
{
    // avoir une convention explicite pour mes règles rego
    // policies/{resource_type}/{action}.rego
    // exemple de fichier JSON 
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
    // Exemple de chemin : path = "/v1/data/policy/{input.resource.type}/{input.action}/allow"
}

// DownloadOpa();
// DiscoverPolicies();
// CompileOpa();
// 1. Implémenter l'éval.
// 2. Implémenter l'API de décision.
// 3. Implémenter le endpoint de discovery.
// 4. 
Eval();
