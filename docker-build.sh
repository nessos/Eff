#!/usr/bin/env bash

IMAGE_LABEL="eff-build-$RANDOM"

# docker build
docker build -t $IMAGE_LABEL .

# dotnet build, test & nuget publish
docker run -t --name $IMAGE_LABEL $IMAGE_LABEL

# copy artifacts
docker cp $IMAGE_LABEL:/artifacts .

# stop container
docker stop $IMAGE_LABEL
