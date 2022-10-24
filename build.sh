#!/bin/bash

scriptdir="$( cd "$(dirname "$0")" ; pwd -P )"

SLN=$scriptdir/source/Meadow.Core.sln

dotnet restore $SLN
dotnet build $SLN