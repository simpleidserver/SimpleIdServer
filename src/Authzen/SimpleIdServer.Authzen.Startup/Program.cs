// See https://aka.ms/new-console-template for more information

using SimpleIdServer.Authzen.Rego;
using SimpleIdServer.Authzen.Rego.Compiler;

static void DownloadOpa()
{
    var downloader = OpaDownloaderFactory.Build();
    downloader.DownloadOpaFile().Wait();
}

static void CompileOpa()
{
    var compilationResult = OpaCompilerFactory.Build().Compile().Result;

}

// DownloadOpa();
CompileOpa();