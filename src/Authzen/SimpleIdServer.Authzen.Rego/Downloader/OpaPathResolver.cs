using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;

namespace SimpleIdServer.Authzen.Rego;

public interface IOpaPathResolver
{
    string ResolveDownloadPath();
    string ResolveOpaFilePath();
    string ResolveOpaFileName();
}

public class OpaPathResolver : IOpaPathResolver
{
    private const string folderName = "download";
    private readonly RegoDownloaderOptions _options;

    public OpaPathResolver(IOptions<RegoDownloaderOptions> options)
    {
        _options = options.Value;
    }
    
    public string ResolveDownloadPath()
    {
        if (string.IsNullOrWhiteSpace(_options.DownloadPath))
        {
            var assemblyLocation = Path.GetDirectoryName(typeof(OpaPathResolver).Assembly.Location);
            return Path.Combine(assemblyLocation!, folderName);
        }

        return _options.DownloadPath;
    }

    public string ResolveOpaFilePath()
    {
        var downloadPath = ResolveDownloadPath();
        var fileName = ResolveOpaFileName();
        return Path.Combine(downloadPath, fileName);
    }

    public string ResolveOpaFileName()
    {
        var (osType, extension) = GetOsInfo();
        var fileName = $"opa_{osType}_amd64{extension}";
        return fileName;
    }

    private static (string osType, string extension) GetOsInfo()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return ("windows", ".exe");
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return ("linux", "");
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return ("darwin", "");
        }

        throw new PlatformNotSupportedException("Current OS platform is not supported");
    }
}
