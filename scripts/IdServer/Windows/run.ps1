#Requires -Version 4.0

function Get-ScriptDirectory
{
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value;
    Split-Path $Invocation.MyCommand.Path;
}

$ErrorActionPreference = "Stop";

$scriptDirectory = Get-ScriptDirectory;
$executable = "SimpleIdServer.IdServer.Startup.exe";
$executableDir = Join-Path $scriptDirectory "Server"
$version = $null;

Push-Location $executableDir;

Try
{
    Invoke-Expression -Command ".\$executable --urls=https://localhost:5001";
    if ($LASTEXITCODE -ne 0) { 
        Read-Host -Prompt "Press enter to continue...";
    }
}
Finally
{
    Pop-Location;
}