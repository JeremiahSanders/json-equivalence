#!/usr/bin/env bash

SCRIPT_DIRECTORY="$( cd "$(dirname "$0")" ; pwd -P )"
PROJECT_ROOT="${SCRIPT_DIRECTORY}/.."
COVERAGE_OUTPUT="${PROJECT_ROOT}/build/coverage/"
VERSION=$(head -n 1 "${PROJECT_ROOT}/VERSION")

cd "${PROJECT_ROOT}"

# Build and test
dotnet build . -c Debug --no-incremental
dotnet test . -p:CollectCoverage=true -p:CoverletOutputFormat=\"json,teamcity\" -p:CoverletOutput="${COVERAGE_OUTPUT}" -p:Exclude=\"[FSharp.Core*]*,[xunit*]*\"

# Pack NuGet package
dotnet pack src --configuration Release --output build/package -p:Version="${VERSION}"
