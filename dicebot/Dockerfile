FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.19 AS build-env
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o output

FROM alpine:3.19
WORKDIR /app
COPY --from=build-env /app/output .
RUN apk update && apk add --no-cache --update libgcc libstdc++ zlib libssl3 libintl icu-libs krb5-libs
ENTRYPOINT ["./dicebot"]
