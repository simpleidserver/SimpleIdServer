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
	
	$Excluded = (".template.config", "template.json", "*.csproj")
	
	if (Test-Path $TargetFolderPath) {	
		Get-ChildItem -Path $TargetFolderPath -Recurse -Exclude $Excluded | Remove-Item -recurse -force
	} else {		
		New-Item $TargetFolderPath -Type Directory
	}
	
	Copy-Item -Path $SourceFolderPath/* -Destination $TargetFolderPath -Exclude *.csproj -recurse -force
}

task default -depends local
task local -depends compile, test
task ci -depends clean, release, local, pack

task clean {
	rd "$source_dir\artifacts" -recurse -force  -ErrorAction SilentlyContinue | out-null
	rd "$base_dir\build" -recurse -force  -ErrorAction SilentlyContinue | out-null
}

task dockerBuild -depends clean {
	$Env:TAG = $suffix
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Startup\SimpleIdServer.IdServer.Startup.csproj -c $config -o $result_dir\docker\IdServer }
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Website.Startup\SimpleIdServer.IdServer.Website.Startup.csproj -c $config -o $result_dir\docker\IdServerWebsite }
	exec { docker-compose build }
}

task dockerUp {
	$Env:TAG = $suffix
	exec { docker-compose up }
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
	$IdServerPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup"
	$IdServerPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup"
	$IdServerWebsitePathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Website.Startup"
	$IdServerWebsitePathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Website.Startup"
	$ScimSQLServerPathSource = "$source_dir/Scim/SimpleIdServer.Scim.SqlServer.Startup"
	$ScimSQLServerPathTarget = "$source_dir/Templates/templates/SimpleIdServer.Scim.SqlServer.Startup"
	$ScimMongoDBPathSource = "$source_dir/Scim/SimpleIdServer.Scim.MongoDb.Startup"
	$ScimMongoDBPathTarget = "$source_dir/Templates/templates/SimpleIdServer.Scim.MongoDb.Startup"
	
	
	CopyFolder $IdServerPathSource $IdServerPathTarget
	CopyFolder $IdServerWebsitePathSource $IdServerWebsitePathTarget
	CopyFolder $ScimSQLServerPathSource $ScimSQLServerPathTarget
	CopyFolder $ScimMongoDBPathSource $ScimMongoDBPathTarget
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