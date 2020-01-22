FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-alpine3.10

RUN apk add git

WORKDIR /repo
COPY . .

CMD dotnet build -c Release && \
    dotnet test --no-build -c Release && \
    dotnet pack -c Release -o artifacts/