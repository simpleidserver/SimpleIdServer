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
	
	$Excluded = (".template.config", "template.json", "*.csproj", "appsettings.json")
	
	if (Test-Path $TargetFolderPath) {	
		Get-ChildItem -Path $TargetFolderPath -Recurse -Exclude $Excluded | Remove-Item -recurse -force
	} else {		
		New-Item $TargetFolderPath -Type Directory
	}
	
	Copy-Item -Path $SourceFolderPath/* -Destination $TargetFolderPath -Exclude $Excluded -recurse -force
}

function GetDockerVersion {
	$env = $env:DOCKER_VERSION
	if($env -eq $NULL) {
		$env = 'ci-local'
	}
	
	return $env	
}

task default -depends local
task local -depends compile, test
task ci -depends clean, release, local, pack, buildInstaller

task fetchSubComponent {
	git submodule update --init --recursive
}

task clean -depends fetchSubComponent {
	rd "$source_dir\artifacts" -recurse -force  -ErrorAction SilentlyContinue | out-null
	rd "$base_dir\build" -recurse -force  -ErrorAction SilentlyContinue | out-null
}

task dockerBuild -depends clean {
	$Env:TAG = GetDockerVersion
	echo "Docker version: $Env:TAG"
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Startup\SimpleIdServer.IdServer.Startup.csproj -c $config -o $result_dir\docker\IdServer }
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Website.Startup\SimpleIdServer.IdServer.Website.Startup.csproj -c $config -o $result_dir\docker\IdServerWebsite }
	exec { dotnet publish $source_dir\Scim\SimpleIdServer.Scim.Startup\SimpleIdServer.Scim.Startup.csproj -c $config -o $result_dir\docker\Scim }
	exec { dotnet publish $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Startup\SimpleIdServer.CredentialIssuer.Startup.csproj -c $config -o $result_dir\docker\CredentialIssuer }
	exec { dotnet publish $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Website.Startup\SimpleIdServer.CredentialIssuer.Website.Startup.csproj -c $config -o $result_dir\docker\CredentialIssuerWebsite }
	exec { docker-compose -f local-docker-compose.yml build --no-cache }
}

task dockerPublish -depends dockerBuild {
	{ exec docker-compose -f local-docker-compose.yml push }
}

task dockerUp {
	$Env:TAG = GetDockerVersion
	exec { docker-compose up }
}

task buildInstaller {
    New-Item $result_dir\windows64\IdServer -Type Directory
    New-Item $result_dir\windows64\IdServerWebsite -Type Directory
    New-Item $result_dir\windows64\Scim -Type Directory
    New-Item $result_dir\windows64\CredentialIssuer -Type Directory
    New-Item $result_dir\windows64\CredentialIssuerWebsite -Type Directory
	
    New-Item $result_dir\linux64\IdServer -Type Directory
    New-Item $result_dir\linux64\IdServerWebsite -Type Directory
    New-Item $result_dir\linux64\Scim -Type Directory
    New-Item $result_dir\linux64\CredentialIssuer -Type Directory
    New-Item $result_dir\linux64\CredentialIssuerWebsite -Type Directory
	
    New-Item $result_dir\docker -Type Directory
	
    New-Item $result_dir\kubernetes -Type Directory
	
	Copy-Item -Path $base_dir\scripts\IdServer\Windows\run.ps1 -Destination $result_dir\windows64\IdServer -force
	Copy-Item -Path $base_dir\scripts\IdServerWebsite\Windows\run.ps1 -Destination $result_dir\windows64\IdServerWebsite -force
	Copy-Item -Path $base_dir\scripts\Scim\Windows\run.ps1 -Destination $result_dir\windows64\Scim -force
	Copy-Item -Path $base_dir\scripts\CredentialIssuer\Windows\run.ps1 -Destination $result_dir\windows64\CredentialIssuer -force
	Copy-Item -Path $base_dir\scripts\CredentialIssuerWebsite\Windows\run.ps1 -Destination $result_dir\windows64\CredentialIssuerWebsite -force
	
	Copy-Item -Path $base_dir\scripts\IdServer\Linux\* -Destination $result_dir\linux64\IdServer -recurse -force
	Copy-Item -Path $base_dir\scripts\IdServerWebsite\Linux\* -Destination $result_dir\linux64\IdServerWebsite -recurse -force
	Copy-Item -Path $base_dir\scripts\Scim\Linux\* -Destination $result_dir\linux64\Scim -recurse -force	
	Copy-Item -Path $base_dir\scripts\CredentialIssuer\Linux\* -Destination $result_dir\linux64\CredentialIssuer -recurse -force
	Copy-Item -Path $base_dir\scripts\CredentialIssuerWebsite\Linux\* -Destination $result_dir\linux64\CredentialIssuerWebsite -recurse -force	

	Copy-Item -Path $base_dir\docker-compose.yml -Destination $result_dir\docker\docker-compose.yml
	Copy-Item -Path $base_dir\compose -Destination $result_dir\docker -recurse -force
	
	Copy-Item -Path $base_dir\sid-kubernetes.yaml -Destination $result_dir\kubernetes -recurse -force
	
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Startup\SimpleIdServer.IdServer.Startup.csproj -c $config -o $result_dir\windows64\IdServer\Server -r win-x64 }
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Website.Startup\SimpleIdServer.IdServer.Website.Startup.csproj -c $config -o $result_dir\windows64\IdServerWebsite\Server -r win-x64 }
	exec { dotnet publish $source_dir\Scim\SimpleIdServer.Scim.Startup\SimpleIdServer.Scim.Startup.csproj -c $config -o $result_dir\windows64\Scim\Server -r win-x64 }
	exec { dotnet publish $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Startup\SimpleIdServer.CredentialIssuer.Startup.csproj -c $config -o $result_dir\windows64\CredentialIssuer\Server -r win-x64 }
	exec { dotnet publish $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Website.Startup\SimpleIdServer.CredentialIssuer.Website.Startup.csproj -c $config -o $result_dir\windows64\CredentialIssuerWebsite\Server -r win-x64 }
	
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Startup\SimpleIdServer.IdServer.Startup.csproj -c $config -o $result_dir\linux64\IdServer\Server -r linux-x64 }
	exec { dotnet publish $source_dir\IdServer\SimpleIdServer.IdServer.Website.Startup\SimpleIdServer.IdServer.Website.Startup.csproj -c $config -o $result_dir\linux64\IdServerWebsite\Server -r linux-x64 }
	exec { dotnet publish $source_dir\Scim\SimpleIdServer.Scim.Startup\SimpleIdServer.Scim.Startup.csproj -c $config -o $result_dir\linux64\Scim\Server -r linux-x64 }
	exec { dotnet publish $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Startup\SimpleIdServer.CredentialIssuer.Startup.csproj -c $config -o $result_dir\linux64\CredentialIssuer\Server -r linux-x64 }
	exec { dotnet publish $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Website.Startup\SimpleIdServer.CredentialIssuer.Website.Startup.csproj -c $config -o $result_dir\linux64\CredentialIssuerWebsite\Server -r linux-x64 }
	
	$compress = @{
	  Path = "$result_dir\windows64\*"
	  CompressionLevel = "Fastest"
	  DestinationPath = "$result_dir\SimpleIdServer-Windows-x64.zip"
	}
	Compress-Archive @compress
	
	$compress = @{
	  Path = "$result_dir\linux64\*"
	  CompressionLevel = "Fastest"
	  DestinationPath = "$result_dir\SimpleIdServer-Linux-x64.zip"
	}
	Compress-Archive @compress
	
	$compress = @{
	  Path = "$result_dir\docker\*"
	  CompressionLevel = "Fastest"
	  DestinationPath = "$result_dir\Docker.zip"
	}
	Compress-Archive @compress
	
	$compress = @{
	  Path = "$result_dir\kubernetes\*"
	  CompressionLevel = "Fastest"
	  DestinationPath = "$result_dir\Kubernetes.zip"
	}
	Compress-Archive @compress
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
	
	exec { dotnet build $base_dir\json-ld.net\JsonLD.sln -c $config --version-suffix=$buildSuffix }
    exec { dotnet build .\SimpleIdServer.IdServer.Host.sln -c $config --version-suffix=$buildSuffix }
    exec { dotnet build .\SimpleIdServer.Scim.Host.sln -c $config --version-suffix=$buildSuffix }
	exec { dotnet build .\SimpleIdServer.Did.sln -c $config --version-suffix=$buildSuffix }
	exec { dotnet build .\SimpleIdServer.CredentialIssuer.Host.sln -c $config --version-suffix=$buildSuffix }
	exec { dotnet build .\SimpleIdServer.Federation.sln -c $config --version-suffix=$buildSuffix }
	exec { dotnet build "$source_dir/Templates/SimpleIdServer.Templates.csproj" -c $config --version-suffix=$buildSuffix }
}

task buildTemplate {
	$IdServerPathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Startup"
	$IdServerPathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Startup"
	$IdServerWebsitePathSource = "$source_dir/IdServer/SimpleIdServer.IdServer.Website.Startup"
	$IdServerWebsitePathTarget = "$source_dir/Templates/templates/SimpleIdServer.IdServer.Website.Startup"
	$ScimPathSource = "$source_dir/Scim/SimpleIdServer.Scim.Startup"
	$ScimPathTarget = "$source_dir/Templates/templates/SimpleIdServer.Scim.Startup"
	$CredentialIssuerPathSource = "$source_dir/CredentialIssuer/SimpleIdServer.CredentialIssuer.Startup"
	$CredentialIssuerPathTarget = "$source_dir/Templates/templates/SimpleIdServer.CredentialIssuer.Startup"
	$CredentialIssuerWebsitePathSource = "$source_dir/CredentialIssuer/SimpleIdServer.CredentialIssuer.Website.Startup"
	$CredentialIssuerWebsitePathTarget = "$source_dir/Templates/templates/SimpleIdServer.CredentialIssuer.Website.Startup"
	
	
	CopyFolder $IdServerPathSource $IdServerPathTarget
	CopyFolder $IdServerWebsitePathSource $IdServerWebsitePathTarget
	CopyFolder $ScimPathSource $ScimPathTarget
	CopyFolder $CredentialIssuerPathSource $CredentialIssuerPathTarget
	CopyFolder $CredentialIssuerWebsitePathSource $CredentialIssuerWebsitePathTarget
}
 
task pack -depends release, compile, buildTemplate {
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.VerifiablePresentation\SimpleIdServer.IdServer.VerifiablePresentation.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer\SimpleIdServer.IdServer.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Domains\SimpleIdServer.IdServer.Domains.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Email\SimpleIdServer.IdServer.Email.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Otp\SimpleIdServer.IdServer.Otp.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Console\SimpleIdServer.IdServer.Console.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Helpers\SimpleIdServer.IdServer.Helpers.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Sms\SimpleIdServer.IdServer.Sms.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Pwd\SimpleIdServer.IdServer.Pwd.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Fido\SimpleIdServer.IdServer.Fido.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Provisioning.LDAP\SimpleIdServer.IdServer.Provisioning.LDAP.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Provisioning.SCIM\SimpleIdServer.IdServer.Provisioning.SCIM.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.DPoP\SimpleIdServer.DPoP.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.Configuration\SimpleIdServer.Configuration.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.Configuration.Redis\SimpleIdServer.Configuration.Redis.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Store.EF\SimpleIdServer.IdServer.Store.EF.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Store.SqlSugar\SimpleIdServer.IdServer.Store.SqlSugar.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Website\SimpleIdServer.IdServer.Website.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.WsFederation\SimpleIdServer.IdServer.WsFederation.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Saml.Idp\SimpleIdServer.IdServer.Saml.Idp.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Saml.Sp\SimpleIdServer.IdServer.Saml.Sp.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.U2FClient\SimpleIdServer.IdServer.U2FClient.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Swagger\SimpleIdServer.IdServer.Swagger.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.SqliteMigrations\SimpleIdServer.IdServer.SqliteMigrations.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.PostgreMigrations\SimpleIdServer.IdServer.PostgreMigrations.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.SqlServerMigrations\SimpleIdServer.IdServer.SqlServerMigrations.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.MySQLMigrations\SimpleIdServer.IdServer.MySQLMigrations.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.OpenIdConnect\SimpleIdServer.OpenIdConnect.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Notification.Fcm\SimpleIdServer.IdServer.Notification.Fcm.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Notification.Gotify\SimpleIdServer.IdServer.Notification.Gotify.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.OpenidFederation\SimpleIdServer.OpenidFederation.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.OpenidFederation.Store.EF\SimpleIdServer.OpenidFederation.Store.EF.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.IdServer.Federation\SimpleIdServer.IdServer.Federation.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.Authority.Federation\SimpleIdServer.Authority.Federation.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\IdServer\SimpleIdServer.Rp.Federation\SimpleIdServer.Rp.Federation.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim\SimpleIdServer.Scim.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Domains\SimpleIdServer.Scim.Domains.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Parser\SimpleIdServer.Scim.Parser.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Client\SimpleIdServer.Scim.Client.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Persistence.EF\SimpleIdServer.Scim.Persistence.EF.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Persistence.MongoDB\SimpleIdServer.Scim.Persistence.MongoDB.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.Swashbuckle\SimpleIdServer.Scim.Swashbuckle.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.SwashbuckleV6\SimpleIdServer.Scim.SwashbuckleV6.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.SqlServerMigrations\SimpleIdServer.Scim.SqlServerMigrations.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.PostgreMigrations\SimpleIdServer.Scim.PostgreMigrations.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Scim\SimpleIdServer.Scim.MySQLMigrations\SimpleIdServer.Scim.MySQLMigrations.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Did\SimpleIdServer.Did\SimpleIdServer.Did.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Did\SimpleIdServer.Did.Ethr\SimpleIdServer.Did.Ethr.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Did\SimpleIdServer.Did.Key\SimpleIdServer.Did.Key.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Did\SimpleIdServer.Vc\SimpleIdServer.Vc.csproj -c $config $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Did\SimpleIdServer.Vp\SimpleIdServer.Vp.csproj -c $config $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer\SimpleIdServer.CredentialIssuer.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Domains\SimpleIdServer.CredentialIssuer.Domains.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Store\SimpleIdServer.CredentialIssuer.Store.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\CredentialIssuer\SimpleIdServer.CredentialIssuer.Website\SimpleIdServer.CredentialIssuer.Website.csproj -c $config --no-build $versionSuffix --output $result_dir }
	exec { dotnet pack $source_dir\Templates\SimpleIdServer.Templates.csproj -c $config --no-build $versionSuffix --output $result_dir }
}

task test {
	Push-Location -Path $base_dir\tests\SimpleIdServer.Configuration.Tests
	
	try {
	    exec { & dotnet test -c $config --no-build --no-restore }
	} finally {
	    Pop-Location
	}
	
	Push-Location -Path $base_dir\tests\SimpleIdServer.CredentialIssuer.Host.Acceptance.Tests
	
	try {
	    exec { & dotnet test -c $config --no-build --no-restore }
	} finally {
	    Pop-Location
	}
	
	Push-Location -Path $base_dir\tests\SimpleIdServer.Did.Ethr.Tests
	
	try {
	    exec { & dotnet test -c $config --no-build --no-restore }
	} finally {
	    Pop-Location
	}
	
	Push-Location -Path $base_dir\tests\SimpleIdServer.Did.Key.Tests
	
	try {
	    exec { & dotnet test -c $config --no-build --no-restore }
	} finally {
	    Pop-Location
	}
	
	Push-Location -Path $base_dir\tests\SimpleIdServer.DID.Tests
	
	try {
	    exec { & dotnet test -c $config --no-build --no-restore }
	} finally {
	    Pop-Location
	}

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

    Push-Location -Path $base_dir\tests\SimpleIdServer.Vc.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }

    Push-Location -Path $base_dir\tests\SimpleIdServer.OpenidFederation.Tests

    try {
        exec { & dotnet test -c $config --no-build --no-restore }
    } finally {
        Pop-Location
    }
}