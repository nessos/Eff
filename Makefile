SOURCE_DIRECTORY := $(dir $(realpath $(lastword $(MAKEFILE_LIST))))

ARTIFACT_PATH := $(SOURCE_DIRECTORY)artifacts
DOCKER_IMAGE_NAME ?= "eff-docker-build"
DOCKER_CMD ?= make pack
NUGET_SOURCE ?= "https://api.nuget.org/v3/index.json"
NUGET_API_KEY ?= ""

clean:
	rm -rf $(ARTIFACT_PATH)/*

build: clean
	dotnet build -c Debug && \
	dotnet build -c Release

test: build
	dotnet test --no-build -c Debug && \
	dotnet test --no-build -c Release

pack: test
	dotnet pack -c Release -o $(ARTIFACT_PATH)

push:
	for nupkg in $(ARTIFACT_PATH)/*.nupkg; do \
		dotnet nuget push -s $(NUGET_SOURCE) -k $(NUGET_API_KEY) --skip-duplicate $$nupkg; \
	done

docker-build: clean
	docker build -t $(DOCKER_IMAGE_NAME) . && \
	docker run --rm -t \
			-v $(ARTIFACT_PATH):/repo/artifacts \
			$(DOCKER_IMAGE_NAME) \
			$(DOCKER_CMD)

	docker rmi -f $(DOCKER_IMAGE_NAME)

.DEFAULT_GOAL := pack