FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-alpine3.9

WORKDIR /repo
COPY . .

CMD dotnet build -c Release && \
    dotnet test --no-build -c Release && \
    dotnet pack -c Release -o artifacts/