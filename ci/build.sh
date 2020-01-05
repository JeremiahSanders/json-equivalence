#!/usr/bin/env bash

SCRIPT_DIRECTORY="$( cd "$(dirname "$0")" ; pwd -P )"
PROJECT_ROOT="${SCRIPT_DIRECTORY}/.."
COVERAGE_OUTPUT="${PROJECT_ROOT}/build/coverage/"
VERSION=$(head -n 1 "${PROJECT_ROOT}/VERSION")
GIT_HASH=$(git log --pretty=format:'%h' -n 1)
BUILD_VERSION="${VERSION}-${GIT_HASH}"

cd "${PROJECT_ROOT}"

# Build and test
dotnet build . -c Debug -p:Version="${BUILD_VERSION}"
dotnet test . -p:CollectCoverage=true -p:CoverletOutputFormat=\"json,teamcity\" -p:CoverletOutput="${COVERAGE_OUTPUT}" -p:Exclude=\"[FSharp.Core*]*,[xunit*]*\"

# Pack NuGet package
dotnet pack src --configuration Release --output build/package -p:Version="${BUILD_VERSION}"
