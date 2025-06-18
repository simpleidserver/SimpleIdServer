using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.Authzen.Dtos;
using SimpleIdServer.Authzen.Rego.Discover;
using SimpleIdServer.Authzen.Services;

namespace SimpleIdServer.Authzen.Rego.Eval;

public class RegoEvaluator : IAuthzenPolicyEvaluator
{
    private readonly RegoOptions _options;
    private readonly IWasmPolicyEvaluator _wasmPolicyEvaluator;
    private readonly ICompiledOpaFilesResolver _compiledOpaFilesResolver;
    private readonly ILogger<RegoEvaluator> _logger;

    public RegoEvaluator(
        IOptions<RegoOptions> options,
        IWasmPolicyEvaluator wasmPolicyEvaluator,
        ICompiledOpaFilesResolver compiledOpaFilesResolver,
        ILogger<RegoEvaluator> logger)
    {
        _options = options.Value;
        _wasmPolicyEvaluator = wasmPolicyEvaluator;
        _compiledOpaFilesResolver = compiledOpaFilesResolver;
        _logger = logger;
    }

    public Task<bool> Evaluate(AccessEvaluationRequest request, CancellationToken cancellationToken)
    {
        var compiledOpaFiles = _compiledOpaFilesResolver.Resolve();
        if (!compiledOpaFiles.Any())
        {
            return Task.FromResult(false);
        }

        var expectedPolicyName = $"{request.Resource.Type}.{request.Action.Name}";
        var selectedCompiledOpaFile = compiledOpaFiles.SingleOrDefault(f => f.PackageName == expectedPolicyName);
        if (selectedCompiledOpaFile == null)
        {
            _logger.LogError($"The policy {expectedPolicyName} doesn't exist");
            return Task.FromResult(false);
        }

        var entryPoint = $"{string.Join("/", selectedCompiledOpaFile.PackageName.Split('.'))}/{Constants.RegoPolicyName}";
        return Task.FromResult(_wasmPolicyEvaluator.EvaluatePolicy(request, selectedCompiledOpaFile.FilePath, entryPoint));
    }
}