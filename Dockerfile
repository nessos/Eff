FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS net8-runtime
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine

COPY --from=net8-runtime /usr/share/dotnet/shared/Microsoft.NETCore.App /usr/share/dotnet/shared/Microsoft.NETCore.App

RUN apk add git make

WORKDIR /repo
COPY . .

CMD make pack