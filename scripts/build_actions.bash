#!/bin/bash

# Exit on any error
set -e

# Get the script's directory and navigate to the root of the repo
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
ACTIONS_DIR="$REPO_ROOT/.github/actions"

echo "Navigating to $ACTIONS_DIR..."
cd "$ACTIONS_DIR"

echo "Installing dependencies..."
npm install

echo "Building GitHub Actions..."
npm run build

echo "GitHub actions successfully built and dist/ folders updated."
