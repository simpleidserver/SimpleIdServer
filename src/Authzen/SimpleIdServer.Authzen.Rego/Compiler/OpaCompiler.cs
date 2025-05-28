using System.Diagnostics;

namespace SimpleIdServer.Authzen.Rego.Compiler;

public interface IOpaCompiler
{
    Task<List<OpaFileCompilationResult>> Compile(CancellationToken cancellationToken = default(CancellationToken));
}

public class OpaFileCompilationResult
{
    public string FilePath
    {
        get; set;
    }

    public bool IsCompiled
    {
        get; set;
    }

    public string ErrorMessage
    {
        get; set;
    }
}

public class OpaCompiler : IOpaCompiler
{
    private readonly IOpaPathResolver _opaPathResolver;
    private readonly IRegoPathResolver _regoPathResolver;

    public OpaCompiler(
        IOpaPathResolver opaPathResolver,
        IRegoPathResolver regoPathResolver)
    {
        _opaPathResolver = opaPathResolver;
        _regoPathResolver = regoPathResolver;
    }

    public async Task<List<OpaFileCompilationResult>> Compile(CancellationToken cancellationToken = default(CancellationToken))
    {
        var opaPath = _opaPathResolver.ResolveOpaFilePath();
        var regoPath = _regoPathResolver.ResolvePoliciesPath();
        var wasmPath = Path.Combine(regoPath, "wasm");
        var compilationResults = new List<(string sourceFile, string targetFile, byte[] compiledContent)>();
        var tmpPath = Path.GetTempPath();
        var regoFiles = Directory.GetFiles(regoPath, "*.rego", SearchOption.AllDirectories);
        var result = new List<OpaFileCompilationResult>();
        foreach (var regoFile in regoFiles)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var fileName = Path.GetFileNameWithoutExtension(regoFile);
            var tempWasmFile = Path.Combine(tmpPath, $"{Guid.NewGuid()}.wasm");
            var finalWasmFile = Path.Combine(wasmPath, $"{fileName}.wasm");
            var startInfo = new ProcessStartInfo
            {
                FileName = opaPath,
                Arguments = $"build -t wasm -e authz/allow \"{regoFile}\" -o \"{tempWasmFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);
            if (process.ExitCode != 0)
            {
                CleanTemporaryFiles(tmpPath);
                result.Add(new OpaFileCompilationResult
                {
                    FilePath = finalWasmFile,
                    IsCompiled = false,
                    ErrorMessage = output
                });
                break;
            }

            var compiledContent = await File.ReadAllBytesAsync(tempWasmFile, cancellationToken);
            compilationResults.Add((regoFile, finalWasmFile, compiledContent));
            try { File.Delete(tempWasmFile); } catch { }
            result.Add(new OpaFileCompilationResult
            {
                FilePath = finalWasmFile,
                IsCompiled = true
            });
        }

        if (Directory.Exists(wasmPath))
        {
            Directory.Delete(wasmPath);
        }

        Directory.CreateDirectory(wasmPath);
        foreach (var (_, targetFile, content) in compilationResults)
        {
            await File.WriteAllBytesAsync(targetFile, content, cancellationToken);
        }

        return result;
    }

    private static void CleanTemporaryFiles(string tmpPath)
    {
        foreach (var tempFile in Directory.GetFiles(tmpPath, "*.wasm"))
        {
            try { File.Delete(tempFile); } catch { }
        }
    }
}