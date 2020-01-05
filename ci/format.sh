#!/usr/bin/env bash

SCRIPT_DIRECTORY="$( cd "$(dirname "$0")" ; pwd -P )"

cd "${SCRIPT_DIRECTORY}/../src" && \
dotnet restore && \
dotnet fantomas . --recurse --indent 4 --pageWidth 120

cd "${SCRIPT_DIRECTORY}/../test" && \
dotnet restore && \
dotnet fantomas . --recurse --indent 4 --pageWidth 120
