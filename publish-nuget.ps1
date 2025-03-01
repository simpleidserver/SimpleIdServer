$directory = "c:\Projects\SimpleIdServer\build\results"
$nugetApiKey = $env:API_KEY
$nugetSource = "https://api.nuget.org/v3/index.json"

Write-Host $nugetApiKey

if (Test-Path -Path $directory) {
    $packages = Get-ChildItem -Path $directory -Filter *.nupkg

    if ($packages.Count -gt 0) {
        foreach ($package in $packages) {
            Write-Host "Publication du package: $($package.FullName)"
            dotnet nuget push $package.FullName -k $nugetApiKey -s $nugetSource
        }
		
        Write-Host "Tous les packages ont été publiés avec succès."
    }
    else {
        Write-Host "Aucun package NuGet trouvé dans le répertoire spécifié."
    }
} else {
    Write-Host "Le répertoire spécifié n'existe pas."
}