#!/bin/bash

if [ "$#" -lt 1 ] || [ "$#" -gt 2 ]; then
    echo "Usage: $0 <component_name> [project_type]"
    exit 1
fi
PROJECT_NAME=InsonusK.Shared
SLN_NAME=DotNet-Shared.sln

COMPONENT_NAME=$1
PROJECT_TYPE=${2:-classlib}
BASE_PATH="src"
COMPONENT_PATH="$BASE_PATH"

CODE_NAME="$PROJECT_NAME.$COMPONENT_NAME"
CODE_PATH="$COMPONENT_PATH/$CODE_NAME"

mkdir -p "$CODE_PATH"

dotnet new "$PROJECT_TYPE" -o "$CODE_PATH" --name "$CODE_NAME"

dotnet sln $SLN_NAME add "$CODE_PATH/$CODE_NAME.csproj"

echo "Projects created and added to solution successfully in $COMPONENT_PATH"