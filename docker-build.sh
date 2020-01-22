#!/usr/bin/env bash

set -e
cd `dirname $0`

IMAGE_LABEL="eff-build-$RANDOM"

# docker build
docker build -t $IMAGE_LABEL .

# dotnet build, test & nuget publish
docker run -t --name $IMAGE_LABEL $IMAGE_LABEL

# copy artifacts
docker cp $IMAGE_LABEL:/repo/artifacts/ .

# stop & remove container
docker stop $IMAGE_LABEL
docker rmi -f $IMAGE_LABEL