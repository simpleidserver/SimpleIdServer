using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;
using SimpleIdServer.Authzen.Rego.Discover;
namespace SimpleIdServer.Authzen.Rego;

public interface IOpaDownloader
{
    Task<string> DownloadOpaFile(CancellationToken cancellationToken = default(CancellationToken));
}

public class OpaDownloader : IOpaDownloader
{
    private readonly IOpaPathResolver _opaPathResolver;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RegoOptions _options;

    public OpaDownloader(
        IOpaPathResolver opaPathResolver,
        IHttpClientFactory httpClientFactory,
        IOptions<RegoOptions> options
    )
    {
        _opaPathResolver = opaPathResolver;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public static string HttpClientName => "Opa";

    public async Task<string> DownloadOpaFile(CancellationToken cancellationToken = default(CancellationToken))
    {
        var downloadPath = _opaPathResolver.ResolveDownloadPath();
        var outputPath = _opaPathResolver.ResolveOpaFilePath();
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        Directory.CreateDirectory(downloadPath);
        await DownloadOpaClient(outputPath);
        await CheckPermissions(outputPath);
        return outputPath;
    }

    private async Task DownloadOpaClient(string outputPath)
    {
        var fileName = _opaPathResolver.ResolveOpaFileName();
        var downloadUrl = $"{_options.GitHubUrl}/{fileName}";
        using (var httpClient = _httpClientFactory.CreateClient(HttpClientName))
        {
            var response = await httpClient.GetAsync(downloadUrl);
            response.EnsureSuccessStatusCode();
            using (var fs = new FileStream(outputPath, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }
        }
    }

    private async Task CheckPermissions(string outputPath)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                await Process.Start("chmod", $"+x {outputPath}").WaitForExitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to set execute permissions: {ex.Message}");
            }
        }
    }
}