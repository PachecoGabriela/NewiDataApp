#!/bin/bash
set -e

# Start PostgreSQL in the background
service postgresql start

# Wait a few seconds to ensure PostgreSQL has started
sleep 5

# Start the .NET application
#exec dotnet iData.Blazor.Server.dll