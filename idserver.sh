#!/usr/bin/env bash

config="debug"

# Getting the bash script executing path. See: https://stackoverflow.com/a/630387
base_dir="$(dirname -- "${BASH_SOURCE[0]}")" # relative
base_dir="$(cd -- "$base_dir" && pwd)"       # absolutized and normalized

if [[ -z "$base_dir" ]]; then
    echo "the path is not accessible"
    exit 1
fi

source_dir="$base_dir/src"
build_dir="$base_dir/build"
result_dir="$build_dir/results"

# Config profile
if [[ -n $2 ]]; then
    config=$2
fi

function getDockerVersion() {
    local docker_version='local'

    if [[ $DOCKER_VERSION != '' ]]; then
        docker_version=$DOCKER_VERSION
    fi

    echo "$docker_version"
}

function fetchSubComponent() {
    git -C "$base_dir" submodule update --init --recursive
}

function clean() {
    fetchSubComponent

    rm -rf "$build_dir"
}

function compile() {
    clean

    dotnet build "$base_dir/json-ld.net/JsonLD.sln" -c "$config" --nologo
    dotnet build "$base_dir/SimpleIdServer.IdServer.Host.sln" -c "$config" --nologo
    dotnet build "$base_dir/SimpleIdServer.Scim.Host.sln" -c "$config" --nologo
    dotnet build "$base_dir/SimpleIdServer.Did.sln" -c "$config" --nologo
    dotnet build "$base_dir/SimpleIdServer.CredentialIssuer.Host.sln" -c "$config" --nologo
}

function setDockerTag() {
    local docker_version
    docker_version=$(getDockerVersion)

    export TAG=$docker_version
}

function dockerBuild {
    clean
    setDockerTag

    echo "Building Docker images with version: $TAG"

    dotnet publish "$source_dir/IdServer/SimpleIdServer.IdServer.Startup/SimpleIdServer.IdServer.Startup.csproj" -c "$config" -o "$result_dir/docker/IdServer" --nologo
    dotnet publish "$source_dir/IdServer/SimpleIdServer.IdServer.Website.Startup/SimpleIdServer.IdServer.Website.Startup.csproj" -c "$config" -o "$result_dir/docker/IdServerWebsite" --nologo
    dotnet publish "$source_dir/Scim/SimpleIdServer.Scim.Startup/SimpleIdServer.Scim.Startup.csproj" -c "$config" -o "$result_dir/docker/Scim" --nologo
    dotnet publish "$source_dir/CredentialIssuer/SimpleIdServer.CredentialIssuer.Startup/SimpleIdServer.CredentialIssuer.Startup.csproj" -c "$config" -o "$result_dir/docker/CredentialIssuer" --nologo
    dotnet publish "$source_dir/CredentialIssuer/SimpleIdServer.CredentialIssuer.Website.Startup/SimpleIdServer.CredentialIssuer.Website.Startup.csproj" -c "$config" -o "$result_dir/docker/CredentialIssuerWebsite" --nologo

    docker compose -f "$base_dir/local-docker-compose.yml" build --no-cache
}

function dockerUp() {
    setDockerTag

    echo "Running Docker containers with version: $TAG"

    docker compose -f "$base_dir/local-docker-compose.yml" up -d
}

function dockerDown() {
    echo "Deleting Docker containers"

    docker compose -f "$base_dir/local-docker-compose.yml" down
}

function dockerStart() {
    echo "Starting Docker containers"

    docker compose -f "$base_dir/local-docker-compose.yml" start
}

function dockerStop() {
    echo "Stopping Docker containers"

    docker compose -f "$base_dir/local-docker-compose.yml" stop
}

function help() {
    echo "Use:
. idserver.sh OPTION CONFIG

If alias idserver was added in ~/.bash_aliases file:

idserver OPTION CONFIG

OPTIONs:
    clean               Clean the folders.
    compile             Compile all the projects.
    dockerBuild         Build all the projects for running them in Docker Containers.
    dockerUp            Build all the projects and run them in Docker Containers.
    dockerDown          Delete all Docker Containers.
    dockerStart         Start all Docker Containers.
    dockerStop          Stop all Docker Containers.
    help                Show this help message.
    
CONFIGs:
    Possible values are: debug and release"
}

# Calling options
case $1 in
"clean")
    clean
    echo "All cleaned"
    ;;
"compile")
    compile
    echo "All compiled"
    ;;
"dockerBuild")
    dockerBuild
    echo "Docker images builded"
    ;;
"dockerUp")
    dockerUp
    echo "Docker containers running"
    ;;
"dockerDown")
    dockerDown
    echo "Docker containers deleted"
    ;;
"dockerStart")
    dockerStart
    echo "Docker containers started"
    ;;
"dockerStop")
    dockerStop
    echo "Docker containers stopped"
    ;;
"fetchSubComponent")
    fetchSubComponent
    echo "Submodules updated"
    ;;
*)
    help
    ;;
esac
