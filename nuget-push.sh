#!/usr/bin/env bash

set -e
cd `dirname $0`

NUGET_SOURCE=https://api.nuget.org/v3/index.json
ARTIFACTS_FOLDER=artifacts/

[ -n $1 ] && NUGET_API_KEY=$1

if [ -z $NUGET_API_KEY ]; then
	echo "missing nuget api key"
	exit 1
fi

if [ ! -d $ARTIFACTS_FOLDER ]; then
	echo "artifacts folder $ARTIFACTS_FOLDER not found"
	exit 1
fi

for nupkg in $ARTIFACTS_FOLDER*.nupkg; do
	dotnet nuget push -s $NUGET_SOURCE -k $NUGET_API_KEY --skip-duplicate $nupkg
done