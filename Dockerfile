FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim as build
ARG PROJECT
WORKDIR /app
COPY . .
RUN dotnet restore ${PROJECT}
RUN dotnet build ${PROJECT} -c Release --no-restore
RUN dotnet publish ${PROJECT} -c Release -o /app/published-app --no-restore --no-build

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS run
ARG ENTRYPOINT
WORKDIR /app
COPY --from=build /app/published-app /app
COPY certs/ca/ca.crt /usr/local/share/ca-certificates/ca.crt
RUN update-ca-certificates
ENTRYPOINT [ "dotnet", "/app/Server.dll" ]