#Requires -Version 4.0

function Get-ScriptDirectory
{
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value;
    Split-Path $Invocation.MyCommand.Path;
}

$ErrorActionPreference = "Stop";

$scriptDirectory = Get-ScriptDirectory;
$executable = "SimpleIdServer.Scim.Startup";
$executableDir = Join-Path $scriptDirectory "Server"
$version = $null;

Push-Location $executableDir;

Try
{
    Invoke-Expression -Command ".\$executable --urls=https://localhost:5003";
    if ($LASTEXITCODE -ne 0) { 
        Read-Host -Prompt "Press enter to continue...";
    }
}
Finally
{
    Pop-Location;
}