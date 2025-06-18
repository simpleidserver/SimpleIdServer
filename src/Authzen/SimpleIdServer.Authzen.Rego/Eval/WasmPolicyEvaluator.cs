using System.Text.Json;
using Microsoft.Extensions.Logging;
using Wasmtime;

namespace SimpleIdServer.Authzen.Rego.Eval;

public interface IWasmPolicyEvaluator
{
    bool EvaluatePolicy(object input, string path, string entryPoint);
}

public class WasmPolicyEvaluator : IWasmPolicyEvaluator
{
    private readonly ILogger<WasmPolicyEvaluator> _logger;

    public WasmPolicyEvaluator(ILogger<WasmPolicyEvaluator> logger)
    {
        _logger = logger;
    }

    public bool EvaluatePolicy(object input, string path, string entryPoint)
    {
        if (!File.Exists(path))
        {
            _logger.LogError($"The wasm file {path} doesn't exist");
            return false;
        }

        var engine = new Engine();
        var module = Module.FromFile(engine, path);
        var opa = new WasmOpaPolicy(engine, module, 2);
        var json = JsonSerializer.Serialize(input);
        var result = opa.EvaluateJson(json, entryPoint, true);
        return true;
    }
}
