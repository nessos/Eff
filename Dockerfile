FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14

RUN apk add git make

WORKDIR /repo
COPY . .

CMD make pack