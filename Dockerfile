FROM mcr.microsoft.com/dotnet/core/sdk:3.1.300-alpine3.11

RUN apk add git make

WORKDIR /repo
COPY . .

CMD make pack