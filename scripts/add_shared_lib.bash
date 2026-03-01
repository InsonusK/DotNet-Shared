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
COMPONENT_PATH="$BASE_PATH/$COMPONENT_NAME"

CODE_NAME="$PROJECT_NAME.$COMPONENT_NAME"
CODE_PATH="$COMPONENT_PATH/$CODE_NAME"
CODE_PROJ_PATH="$CODE_PATH/$CODE_NAME.csproj"

TEST_NAME="$CODE_NAME.Test"
TEST_PATH="$COMPONENT_PATH/$TEST_NAME"  
TEST_PROJ_PATH="$TEST_PATH/$TEST_NAME.csproj"

mkdir -p "$CODE_PATH"
mkdir -p "$TEST_PATH" 

dotnet new "$PROJECT_TYPE" -o "$CODE_PATH" --name "$CODE_NAME"
dotnet new xunit -o "$TEST_PATH" --name "$TEST_NAME"  
dotnet add $TEST_PROJ_PATH reference $CODE_PROJ_PATH

dotnet sln $SLN_NAME add $CODE_PROJ_PATH
dotnet sln $SLN_NAME add $TEST_PROJ_PATH

echo "Projects created and added to solution successfully in $COMPONENT_PATH"