using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
using SimpleIdServer.Authzen.Rego.Discover;
using YamlDotNet.Serialization.ValueDeserializers;

namespace SimpleIdServer.Authzen.Rego.Compiler;

public interface IOpaCompiler
{
    Task<List<OpaFileCompilationResult>> Compile(CancellationToken cancellationToken = default(CancellationToken));
}

public class OpaFileCompilationResult
{
    public string? FilePath
    {
        get; set;
    }

    public bool IsCompiled
    {
        get; set;
    }

    public string? ErrorMessage
    {
        get; set;
    }
}

public class OpaCompiler : IOpaCompiler
{
    private readonly IOpaPathResolver _opaPathResolver;
    private readonly IRegoPoliciesResolver _regoPoliciesResolver;
    private readonly ICompiledOpaFilesResolver _compiledOpaFilesResolver;

    public OpaCompiler(
        IOpaPathResolver opaPathResolver,
        IRegoPoliciesResolver regoPoliciesResolver,
        ICompiledOpaFilesResolver compiledOpaFilesResolver)
    {
        _opaPathResolver = opaPathResolver;
        _regoPoliciesResolver = regoPoliciesResolver;
        _compiledOpaFilesResolver = compiledOpaFilesResolver;
    }

    public async Task<List<OpaFileCompilationResult>> Compile(CancellationToken cancellationToken = default(CancellationToken))
    {
        var opaPath = _opaPathResolver.ResolveOpaFilePath();
        var policies = await _regoPoliciesResolver.Discover(cancellationToken);
        var wasmPath = _compiledOpaFilesResolver.GetCompiledOpaFolderPath();
        var tmpPath = Path.GetTempPath();
        var result = new List<OpaFileCompilationResult>();
        var compilationResults = new List<(string sourceFile, string targetFile, byte[] compiledContent)>();
        if (policies == null)
        {
            return result;
        }
        
        foreach (var policy in policies)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var fileName = Path.GetFileNameWithoutExtension(policy.Path);
            var tempWasmFile = Path.Combine(tmpPath, $"{Guid.NewGuid()}.{Constants.GzExtension}");
            var finalWasmFile = Path.Combine(wasmPath, $"{policy.PolicyName}.{Constants.WasmFileExtension}");
            var args = $"build -t wasm -e {policy.AllowPolicyName} {string.Join(" ", policy.Libs.Select(l => $"\"{l.filePath}\""))} \"{policy.Path}\" -o \"{tempWasmFile}\"";
            var startInfo = new ProcessStartInfo
            {
                FileName = opaPath,
                Arguments = $"build -t wasm -e {policy.AllowPolicyName} {string.Join(" ", policy.Libs.Select(l => $"\"{l.filePath}\""))} \"{policy.Path}\" -o \"{tempWasmFile}\"",
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

            var compiledContent = await ExtractWasmFile(tempWasmFile);
            compilationResults.Add((policy.Path, finalWasmFile, compiledContent));
            try { File.Delete(tempWasmFile); } catch { }
            result.Add(new OpaFileCompilationResult
            {
                FilePath = finalWasmFile,
                IsCompiled = true
            });
        }

        if (Directory.Exists(wasmPath))
        {
            Directory.Delete(wasmPath, true);
        }

        Directory.CreateDirectory(wasmPath);
        foreach (var (_, targetFile, content) in compilationResults)
        {
            await File.WriteAllBytesAsync(targetFile, content, cancellationToken);
        }

        File.Copy(
            Path.Combine(_regoPoliciesResolver.GetPolicyPath(), Constants.PolicyFileName),
            Path.Combine(wasmPath, Constants.PolicyFileName));
        return result;
    }

    private async Task<byte[]> ExtractWasmFile(string gzFilePath)
    {
        var outputDirectory = Path.Combine(Path.GetDirectoryName(gzFilePath) ?? string.Empty, Path.GetFileNameWithoutExtension(gzFilePath));
        Directory.CreateDirectory(outputDirectory);
        var wasmOutputFile = Path.Combine(outputDirectory, "policy.wasm");
        using (var gzStream = File.OpenRead(gzFilePath))
        using (var gzip = new GZipStream(gzStream, CompressionMode.Decompress))
        using (var reader = new TarReader(gzip))
        {
            TarEntry entry;
            while ((entry = reader.GetNextEntry()) != null)
            {
                if (entry.Name.EndsWith("policy.wasm", StringComparison.OrdinalIgnoreCase))
                {
                    using (var outStream = File.Create(wasmOutputFile))
                    {
                        await entry.DataStream.CopyToAsync(outStream);
                    }
                    break;
                }
            }
        }

        return await File.ReadAllBytesAsync(wasmOutputFile);
    }

    private static void CleanTemporaryFiles(string tmpPath)
    {
        foreach (var tempFile in Directory.GetFiles(tmpPath, "*.wasm"))
        {
            try { File.Delete(tempFile); } catch { }
        }
    }
}