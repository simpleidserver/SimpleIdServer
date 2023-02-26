properties {
	$base_dir = resolve-path .
	$build_dir = "$base_dir\build"
	$source_dir = "$base_dir\src"
	$result_dir = "$build_dir\results"
	$global:config = "debug"
	$tag = $(git tag -l --points-at HEAD)
	$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
	$suffix = @{ $true = ""; $false = "ci-$revision"}[$tag -ne $NULL -and $revision -ne "local"]
	$commitHash = $(git rev-parse --short HEAD)
	$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]
    $versionSuffix = @{ $true = "--version-suffix=$($suffix)"; $false = ""}[$suffix -ne ""]
}

function CopyFolder{
	param(
		$SourceFolderPath,
		$TargetFolderPath
	)
	
	if (Test-Path $TargetFolderPath) {	
		Remove-Item $TargetFolderPath -recurse -force
	}
	
	New-Item $TargetFolderPath -Type Directory
	
	Copy-Item $SourceFolderPath/* $TargetFolderPath -recurse -force
}

task default -depends local
task local -depends compile, test
task ci -depends clean, release, local, pack

task clean {
	rd "$source_dir\artifacts" -recurse -force  -ErrorAction SilentlyContinue | out-null
	rd "$base_dir\build" -recurse -force  -ErrorAction SilentlyContinue | out-null
}

task publishDocker -depends clean {
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Startup\SimpleIdServer.IdServer.Startup.csproj -c $config -o $result_dir\docker\IdServer }
	exec { docker-compose build }
}

task release {
    $global:config = "release"
}

task compile -depends clean {
	echo "build: Tag is $tag"
	echo "build: Package version suffix is $suffix"
	echo "build: Build version suffix is $buildSuffix" 
	
	exec { dotnet --version }
	exec { dotnet --info }

	exec { msbuild -version }
	
    exec { dotnet build .\SimpleIdServer.IdServer.Host.sln -c $config --version-suffix=$buildSuffix }
    exec { dotnet build .\SimpleIdServer.Scim.Host.sln -c $config --version-suffix=$buildSuffix }
	exec { dotnet build "$source_dir/Templates/SimpleIdServer.Templates.csproj" -c $config --version-suffix=$buildSuffix }
}

task buildTemplate {
	$AreasPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/Areas"
	$AreasPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup/Areas"
	$HelpersPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/Helpers"
	$HelpersPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup/Helpers"
	$MigrationsPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/Migrations"
	$MigrationsPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup/Migrations"
	$ResourcesPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/Resources"
	$ResourcesPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup/Resources"
	$ViewsPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/Views"
	$ViewsPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup/Views"
	$ConvertersPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/Converters"
	$ConvertersPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup/Converters"
	$ImagesPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/wwwroot/images"
	$ImagesPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup/wwwroot/images"
	$StylesPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/wwwroot/styles"
	$StylesPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup/wwwroot/styles"	
	$PagesPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Website.Startup/Pages"
	$PagesPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Website.Startup/Pages"
	
	
	CopyFolder $AreasPathSource $AreasPathTarget
	CopyFolder $HelpersPathSource $HelpersPathTarget
	CopyFolder $MigrationsPathSource $MigrationsPathTarget
	CopyFolder $ResourcesPathSource $ResourcesPathTarget
	CopyFolder $ViewsPathSource $ViewsPathTarget
	CopyFolder $ImagesPathSource $ImagesPathTarget
	CopyFolder $StylesPathSource $StylesPathTarget
	CopyFolder $ConvertersPathSource $ConvertersPathTarget
	CopyFolder $PagesPathSource $PagesPathTarget
}
 
task pack -depends release, compile, buildTemplate {
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer\SimpleIdServer.IdServer.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Domains\SimpleIdServer.IdServer.Domains.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Email\SimpleIdServer.IdServer.Email.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Helpers\SimpleIdServer.IdServer.Helpers.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Sms\SimpleIdServer.IdServer.Sms.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Store\SimpleIdServer.IdServer.Store.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Website\SimpleIdServer.IdServer.Website.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.WsFederation\SimpleIdServer.IdServer.WsFederation.csproj -c $config --no-build $versionSuffix --output $result_dir }	
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim\SimpleIdServer.Scim.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Domains\SimpleIdServer.Scim.Domains.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Parser\SimpleIdServer.Scim.Parser.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Persistence.EF\SimpleIdServer.Scim.Persistence.EF.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Persistence.MongoDB\SimpleIdServer.Scim.Persistence.MongoDB.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Swashbuckle\SimpleIdServer.Scim.Swashbuckle.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.SwashbuckleV6\SimpleIdServer.Scim.SwashbuckleV6.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Templates\SimpleIdServer.Templates.csproj -c $config --no-build $versionSuffix --output $result_dir }
}

task test {
    Push-Location -Path $base_dir\tests\SimpleIdServer.IdServer.Host.Acceptance.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }

    Push-Location -Path $base_dir\tests\SimpleIdServer.IdServer.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }

    Push-Location -Path $base_dir\tests\SimpleIdServer.Scim.Host.Acceptance.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }

    Push-Location -Path $base_dir\tests\SimpleIdServer.Scim.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }
}