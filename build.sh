#!/bin/bash

# Exit if any command fails
set -e

# Print commands for debugging
set -x

# Install .NET SDK
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 7.0.100

# Export .NET installation to the PATH
export PATH="$PATH:/opt/buildhome/.dotnet"

# Restore and build the project
dotnet restore
dotnet publish -c Release -o out

# Kill any remaining processes
pkill -f dotnet
