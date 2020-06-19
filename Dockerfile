FROM mcr.microsoft.com/dotnet/core/sdk:3.1.300-alpine3.11

RUN apk add git

WORKDIR /repo
COPY . .

CMD dotnet build -c Release && \
    dotnet test -c Debug && \
    dotnet test --no-build -c Release && \
    dotnet pack -c Release -o artifacts/